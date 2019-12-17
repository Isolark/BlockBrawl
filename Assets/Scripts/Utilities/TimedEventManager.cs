using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : MonoBehaviour 
{
    public BaseController BaseController;

    public float TimeStep;

    private IList<TimedAction> ActionList;
    private IList<TimedAction> DeletionList;
    private float TimePaused;
    private float LastTime;

    void Start()
    {
        ActionList = new List<TimedAction>();
        DeletionList = new List<TimedAction>();
        Unpause();
    }

    public void Pause()
    {
        StopCoroutine(TimedUpdate());

        TimePaused = Time.timeSinceLevelLoad;
    }

    public void Unpause()
    {
        StartCoroutine(TimedUpdate());

        LastTime = Time.timeSinceLevelLoad;
    }

    public void AddTimedAction(Action action, float activationTime)
    {
        ActionList.Add(new TimedAction(action, activationTime));
    }

    //Main Coroutine
    private IEnumerator TimedUpdate()
    {
        for(;;)
        {
            yield return new WaitForSeconds(TimeStep);
            
            var timeDelta = Time.timeSinceLevelLoad - LastTime;
            LastTime = Time.timeSinceLevelLoad;

            foreach(var action in ActionList)
            {
                if(action.TickDown(timeDelta)) {
                    DeletionList.Add(action);
                }
            }
            foreach(var actionToDelete in DeletionList)
            {
                ActionList.Remove(actionToDelete);
            }
            if(DeletionList.Count > 0) {
                DeletionList.Clear();
            }
        }
    }
}

public class TimedAction
{
    private float ActivationTime;
    private Action MyAction;

    public TimedAction(Action myAction, float activationTime)
    {
        MyAction = myAction;
        ActivationTime = activationTime;
    }

    public bool TickDown(float timeDelta)
    {
        ActivationTime -= timeDelta;

        if(ActivationTime <= 0) {
            MyAction();
            return true;
        }
        return false;
    }
}