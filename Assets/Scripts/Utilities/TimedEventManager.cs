using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimedEventManager : MonoBehaviour 
{
    public BaseController BaseController;

    public float TimeStep;

    private IList<TimedAction> ActionList;
    private IList<TimedAction> StagingList;
    private IList<TimedAction> DeletionList;
    private float TimePaused;
    private float LastTime;

    void Start()
    {
        ActionList = new List<TimedAction>();
        StagingList = new List<TimedAction>();
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

    public TimedAction AddTimedAction(Action action, float activationTime)
    {
        var timedAction = new TimedAction(action, activationTime);
        StagingList.Add(timedAction);

        return timedAction;
    }

    //Main Coroutine
    private IEnumerator TimedUpdate()
    {
        for(;;)
        {
            yield return new WaitForSeconds(TimeStep);
            
            var timeDelta = Time.timeSinceLevelLoad - LastTime;
            LastTime = Time.timeSinceLevelLoad;

            if(StagingList.Count > 0) {
                foreach(var action in StagingList) {
                    ActionList.Add(action);
                }
                StagingList.Clear();
            }
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
        }
    }
}

public class TimedAction
{
    public float ActivationTime;
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