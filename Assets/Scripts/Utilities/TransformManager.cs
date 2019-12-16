using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformManager : MonoBehaviour 
{
    public BaseController BaseController;

    private IList<TransformItem> TransformItemList;
    private IList<TransformItem> DeletionList;

    private bool Paused;

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
        BaseController.FixedUpdateDelegate -= OnFixedUpdate;
        Paused = true;
    }

    public void Unpause()
    {
        BaseController.FixedUpdateDelegate += OnFixedUpdate;
        Paused = false;
    }

    public void AddTimedPositionTransform(GameObject target, Vector2 pFinal, float timeDelta, Action callback = null)
    {
        TransformItemList.Add(new TransformItem(target, pFinal, timeDelta, callback));
    }

    

    private void OnFixedUpdate()
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

public class TransformItem
{
    private GameObject Target;
    private Vector2 P_Final;
    private Vector2 P_Velocity;
    private Vector2 P_Acceleration;
    private Action Callback;

    public TransformItem(GameObject target, Vector2 pFinal, Vector2 pVeloc, Vector2 pAccel)
    {
        Target = target;
        P_Final = pFinal;
        P_Velocity = pVeloc;
        P_Acceleration = pAccel;
    }

    public TransformItem(GameObject target, Vector2 pFinal, float timeDelta, Action callback = null)
    {
        Target = target;
        P_Final = pFinal;

        //Calculate (Linear) Velocity
        var pCurrent = new Vector2(target.transform.localPosition.x, target.transform.localPosition.y);
        var xVelocity = (pFinal.x - pCurrent.x) / timeDelta;
        var yVelocity = (pFinal.y - pCurrent.y) / timeDelta;

        P_Velocity = new Vector2(xVelocity, yVelocity);

        Callback = callback;
    }

    public bool OnMove()
    {
        var deltaVector = new Vector3(P_Velocity.x * Time.fixedDeltaTime, P_Velocity.y * Time.fixedDeltaTime, 0);
        var nextVector = Target.transform.localPosition + deltaVector;

        if(P_Velocity.x > 0 && nextVector.x >= P_Final.x || 
            P_Velocity.x < 0 && nextVector.x <= P_Final.x ||
            P_Velocity.y > 0 && nextVector.y >= P_Final.y ||
            P_Velocity.y < 0 && nextVector.y <= P_Final.y) 
        {
            Target.transform.localPosition = P_Final;
            if(Callback != null) {
                Callback();
            }
            return true;
        }

        Target.transform.localPosition = nextVector;

        return false;
    }
}