// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class TransformManager : MonoBehaviour 
// {
//     public MainController MainCtrl;

//     private IList<TransformItem> TransformItemList;
//     private IList<TransformItem> DeletionList;

//     private bool Paused;
//     private float PrevTime;

//     void Start()
//     {
//         TransformItemList = new List<TransformItem>();
//         DeletionList = new List<TransformItem>();
//         Unpause();
//     }

//     public void Pause()
//     {
//         MainCtrl.FixedUpdateDelegate -= OnUpdate;
//         Paused = true;
//     }

//     public void Unpause()
//     {
//         MainCtrl.FixedUpdateDelegate += OnUpdate;
//         Paused = false;
//     }

//     public void Reset()
//     {
//         TransformItemList.Clear();
//         DeletionList.Clear();
//     }

//     public void Add_LinearTimePos_Transform(GameObject target, Vector2 pFinal, float timeDelta, Action callback = null)
//     {
//         TransformItemList.Add(new LinearTimeTransItem(target, pFinal, timeDelta, callback:callback));
//     }

//     public void Add_ManualFinalPos_Transform(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Action callback = null)
//     {
//         TransformItemList.Add(new ManualTransItem(target, pFinal, pVelocity, pAcceleration, callback:callback));
//     }

//     public void Add_ManualDeltaPos_Transform(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pMaxVelocity, Vector2 pAcceleration, 
//         Func<bool> checkCallback, IList<GameObject> linkedObjs = null, Action callback = null)
//     {
//         var transItem = new ManualDeltaTransItem(target, pFinal, pVelocity, pAcceleration, pMaxVelocity, checkCallback, callback:callback);
//         if(linkedObjs != null) {
//             transItem.SetLinkedObjs(linkedObjs); 
//         }

//         TransformItemList.Add(transItem);
//     }

//     public void Add_AccelToPos_Transform(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, float pMarginFinal, Action callback = null)
//     {
//         var transItem = new AccelToPosTransItem(target, pFinal, pVelocity, pAcceleration, pMarginFinal, callback);
//         TransformItemList.Add(transItem);
//     }

//     private void OnUpdate()
//     {
//         if(Paused) return;

//         foreach(var item in TransformItemList)
//         {
//             if(item.OnMove()) {
//                 DeletionList.Add(item);
//             }
//         }
//         foreach(var itemToDelete in DeletionList)
//         {
//             TransformItemList.Remove(itemToDelete);
//         }
//         if(DeletionList.Count > 0) {
//             DeletionList.Clear();
//         }
//     }
// }

// public class AccelToPosTransItem: ManualTransItem
// {
//     public AccelToPosTransItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, float pMarginFinal, Action callback = null)
//         : base(target, pFinal, pVelocity, pAcceleration, callback)
//     {
//         SetPosMargin(pMarginFinal, false);
//     }

//     override public bool OnMove()
//     {
//         var t = Time.deltaTime;
//         var xAccel = Mathf.Lerp(Target.transform.localPosition.x, P_Final.x, t) * P_Acceleration.x;
//         var yAccel = Mathf.Lerp(Target.transform.localPosition.y, P_Final.y, t) * P_Acceleration.y;

//         P_Velocity += new Vector2(xAccel, yAccel);

//         return OnMove(P_Velocity * t, Vector2.zero);
//     }
// } 

// public class ManualDeltaTransItem : ManualTransItem
// {
//     private Vector2 P_Delta;
//     private Func<bool> CheckCallback;

//     public ManualDeltaTransItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Func<bool> checkCallback = null, Action callback = null) 
//         : base(target, pFinal, pVelocity, pAcceleration, callback)
//     {
//         P_Delta = pFinal;
//         P_Final = new Vector2(target.transform.localPosition.x, target.transform.localPosition.y) + P_Delta;
//         CheckCallback = checkCallback;
//     }

//     public ManualDeltaTransItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Vector2 pMaxVelocity, 
//         Func<bool> checkCallback = null, Action callback = null) : base(target, pFinal, pVelocity, pAcceleration, pMaxVelocity, callback)
//     {
//         P_Delta = pFinal;
//         P_Final = new Vector2(target.transform.localPosition.x, target.transform.localPosition.y) + P_Delta;
//         CheckCallback = checkCallback;
//     }

//     override public bool OnMove()
//     {
//         var t = Time.deltaTime;
//         var deltaVector = new Vector3(P_Velocity.x * t, P_Velocity.y * t, 0);
//         var deltaAcceleration = P_Acceleration * t;

//         while(deltaVector.sqrMagnitude / P_Delta.sqrMagnitude > 1)
//         {
//             if(OnMove(P_Delta, Vector2.zero)) { return true; }
//             deltaVector = deltaVector - new Vector3(P_Delta.x, P_Delta.y, 0);
//         }

//         return OnMove(deltaVector, deltaAcceleration);
//     }

//     override protected bool OnCheck()
//     {
//         var checkBool = CheckCallback();
//         if(!checkBool) { P_Final = P_Final + P_Delta; }

//         return checkBool;
//     }
// }

// public class ManualTransItem : TransformItem
// {
//     protected Vector2 P_Velocity;
//     protected Vector2 P_MaxVelocity;
//     protected Vector2 P_Acceleration;

//     protected float P_MarginFinal;
//     protected bool IsForcedMargin;

//     public ManualTransItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Action callback = null)
//         : base(target, pFinal, callback)
//     {
//         P_Velocity = pVelocity;
//         P_Acceleration = pAcceleration;
//         P_MarginFinal = 0;
//         IsForcedMargin = false;
//     }

//     public ManualTransItem(GameObject target, Vector2 pFinal, Vector2 pVelocity, Vector2 pAcceleration, Vector2 pMaxVelocity, Action callback = null)
//         : base(target, pFinal, callback)
//     {
//         P_Velocity = pVelocity;
//         P_MaxVelocity = pMaxVelocity;
//         P_Acceleration = pAcceleration;
//         P_MarginFinal = 0;
//         IsForcedMargin = false;
//     }

//     public void SetPosMargin(float pMarginFinal, bool isForcedMargin = false)
//     {
//         P_MarginFinal = pMarginFinal;
//         IsForcedMargin = isForcedMargin;
//     }

//     protected bool OnMove(Vector3 deltaVector, Vector2 deltaAcceleration)
//     {
//         var nextVector = Target.transform.localPosition + deltaVector;

//         if(P_MarginFinal != 0 && Mathf.Abs(Vector2.Distance(nextVector, P_Final)) <= P_MarginFinal)
//         {
//             FinalMove(deltaVector);
//         }
//         else if(!IsForcedMargin &&
//             P_Velocity.x > 0 && nextVector.x >= P_Final.x || 
//             P_Velocity.x < 0 && nextVector.x <= P_Final.x ||
//             P_Velocity.y > 0 && nextVector.y >= P_Final.y ||
//             P_Velocity.y < 0 && nextVector.y <= P_Final.y) 
//         {
//             FinalMove(deltaVector);
//         }

//         IncPosition(deltaVector);

//         //Increase Velocity
//         P_Velocity += deltaAcceleration;

//         if(P_MaxVelocity != null && P_Velocity.sqrMagnitude > P_MaxVelocity.sqrMagnitude) {
//             P_Velocity = P_MaxVelocity;
//             P_Acceleration = Vector2.zero;
//         }

//         return false;
//     }

//     override public bool OnMove()
//     {
//         var t = Time.deltaTime;
//         var deltaVector = new Vector3(P_Velocity.x * t, P_Velocity.y * t, 0);
//         var deltaAcceleration = P_Acceleration * t;

//         return OnMove(deltaVector, deltaAcceleration);
//     }

//     protected bool FinalMove(Vector3 deltaVector)
//     {
//         if(OnCheck())
//         {
//             deltaVector = P_Final - new Vector2(Target.transform.localPosition.x, Target.transform.localPosition.y);
//             IncPosition(deltaVector);

//             if(Callback != null) {
//                 Callback();
//             }
//             return true;
//         }
//         return false;
//     }

//     virtual protected bool OnCheck() { return true; }
// }

// //NOTE: Currently doesn't support linked obj movement
// public class LinearTimeTransItem : TransformItem
// {
//     // time.Delta is added to T_Current until reaches T_Final
//     protected float T_Final;
//     protected float T_Current;

//     public LinearTimeTransItem(GameObject target, Vector2 pFinal, float tFinal, Action callback = null) : base(target, pFinal, callback)
//     {
//         T_Final = tFinal;
//         T_Current = 0;
//     }

//     override public bool OnMove()
//     {
//         T_Current += Time.deltaTime;

//         if(T_Current >= T_Final) {
//             Target.transform.localPosition = P_Final;
//             if(Callback != null) {
//                 Callback();
//             }
//             return true;
//         }

//         var t = T_Current / T_Final;
        
//         var pNext = Vector2.Lerp(Target.transform.localPosition, P_Final, t);
//         Target.transform.localPosition = pNext;

//         return false;
//     }
// }

// //Is only for "Timed" Transform (consider making another sibling class if need non-timed)
// public abstract class TransformItem
// {
//     protected GameObject Target;
//     protected IList<GameObject> LinkedObjs;
//     protected Vector2 P_Final;
//     protected Action Callback;

//     public TransformItem(GameObject target, Vector2 pFinal, Action callback = null)
//     {
//         Target = target;
//         P_Final = pFinal;

//         Callback = callback;
//     }

//     virtual public void SetLinkedObjs(IList<GameObject> linkedObjs)
//     {
//         LinkedObjs = linkedObjs;
//     }

//     virtual public void IncPosition(Vector2 pDelta2)
//     {
//         var pDelta = new Vector3(pDelta2.x, pDelta2.y, 0);
//         Target.transform.localPosition += pDelta;

//         if(LinkedObjs != null) {
//             foreach(var obj in LinkedObjs) {
//                 obj.transform.localPosition += pDelta;
//             }
//         }
//     }
//     abstract public bool OnMove();
// }