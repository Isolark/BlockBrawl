using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformManager : MonoBehaviour 
{
    public BaseController BaseController;

    private IList<TransformItem> TransformItemList;
    private IList<TransformItem> DeletionList;

    private bool Paused;
    private float PrevTime;

    void Start()
    {
        TransformItemList = new List<TransformItem>();
        DeletionList = new List<TransformItem>();
        Unpause();
    }

    void Destroy()
    {
        Pause();
    }

    public void Pause()
    {
        BaseController.FixedUpdateDelegate -= OnUpdate;
        Paused = true;
    }

    public void Unpause()
    {
        BaseController.FixedUpdateDelegate += OnUpdate;
        Paused = false;
    }

    public void Add_TimedLinearPos_Transform(GameObject target, Vector2 pFinal, float timeDelta, Action callback = null)
    {
        TransformItemList.Add(new LinearTransformItem(target, pFinal, timeDelta, callback:callback));
    }

    public void Add_ManualPos_Transform(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Action callback = null)
    {
        TransformItemList.Add(new ManualTransformItem(target, pFinal, pVelocity, pAcceleration, callback:callback));
    }

    public void Add_InfManualPos_Transform(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Func<bool> checkCallback, Action callback = null)
    {
        TransformItemList.Add(new InfManualTransformItem(target, pFinal, pVelocity, pAcceleration, checkCallback, callback:callback));
    }

    private void OnUpdate()
    {
        if(Paused) return;

        foreach(var item in TransformItemList)
        {
            if(item.OnMove()) {
                DeletionList.Add(item);
            }
        }
        foreach(var itemToDelete in DeletionList)
        {
            TransformItemList.Remove(itemToDelete);
        }
        if(DeletionList.Count > 0) {
            DeletionList.Clear();
        }
    }
}



public class InfManualTransformItem : ManualTransformItem
{
    private Func<bool> CheckCallback;

    public InfManualTransformItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Func<bool> checkCallback = null, Action callback = null) 
        : base(target, pFinal, pVelocity, pAcceleration, callback)
    {
        CheckCallback = checkCallback;
    }

    override protected bool OnCheck()
    {
        return CheckCallback();
    }
}

public class ManualTransformItem : TransformItem
{
    protected Vector2 P_Velocity;
    protected Vector2 P_Acceleration;

    public ManualTransformItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Action callback = null)
        : base(target, pFinal, callback)
    {
        P_Velocity = pVelocity;
        P_Acceleration = pAcceleration;
    }

    override public bool OnMove()
    {
        var t = Time.deltaTime;
        var deltaVector = new Vector3(P_Velocity.x * t, P_Velocity.y * t, 0);
        var nextVector = Target.transform.localPosition + deltaVector;

        if(P_Velocity.x > 0 && nextVector.x >= P_Final.x || 
            P_Velocity.x < 0 && nextVector.x <= P_Final.x ||
            P_Velocity.y > 0 && nextVector.y >= P_Final.y ||
            P_Velocity.y < 0 && nextVector.y <= P_Final.y) 
        {
            if(OnCheck())
            {
                Target.transform.localPosition = P_Final;
                if(Callback != null) {
                    Callback();
                }
                return true;
            }
        }

        Target.transform.localPosition = nextVector;
        P_Velocity += P_Acceleration * t;

        return false;
    }

    virtual protected bool OnCheck() { return true; }
}

public class LinearTransformItem : TransformItem
{
    // T_Current is added to by time.Delta until reaches T_Final
    protected float T_Final;
    protected float T_Current;

    public LinearTransformItem(GameObject target, Vector2 pFinal, float tFinal, Action callback = null) : base(target, pFinal, callback)
    {
        T_Final = tFinal;
        T_Current = 0;
    }

    override public bool OnMove()
    {
        T_Current += Time.deltaTime;

        if(T_Current >= T_Final) {
            Target.transform.localPosition = P_Final;
            if(Callback != null) {
                Callback();
            }
            return true;
        }

        var t = T_Current / T_Final;
        
        var pNext = Vector2.Lerp(Target.transform.localPosition, P_Final, t);
        Target.transform.localPosition = pNext;

        return false;
    }
}

//Is only for "Timed" Transform (consider making another sibling class if need non-timed)
public abstract class TransformItem
{
    protected GameObject Target;
    protected Vector2 P_Final;
    protected Action Callback;

    public TransformItem(GameObject target, Vector2 pFinal, Action callback = null)
    {
        Target = target;
        P_Final = pFinal;

        Callback = callback;
    }

    abstract public bool OnMove();
}