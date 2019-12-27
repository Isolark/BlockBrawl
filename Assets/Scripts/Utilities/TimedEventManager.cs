using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : MonoBehaviour 
{
    public BaseController BaseController;

    private IList<TimedAction> ActionList;
    private IList<TimedAction> StagingList;
    private IList<TimedAction> DeletionList;
    private bool Paused;
    private float PrevTime;

    void Start()
    {
        ActionList = new List<TimedAction>();
        StagingList = new List<TimedAction>();
        DeletionList = new List<TimedAction>();
        Unpause();
    }

    void Destroy()
    {
        Pause();
    }

    public void Pause()
    {
        BaseController.UpdateDelegate -= OnUpdate;
        Paused = true;
    }

    public void Unpause()
    {
        BaseController.UpdateDelegate += OnUpdate;
        Paused = false;
    }

    public TimedAction AddTimedAction(Action action, float activationTime)
    {
        var timedAction = new TimedAction(action, activationTime);
        StagingList.Add(timedAction);

        return timedAction;
    }

    private void OnUpdate()
    {
        if(Paused) { return; }

        if(StagingList.Count > 0) {
            foreach(var action in StagingList) {
                ActionList.Add(action);
            }
            StagingList.Clear();
        }
        foreach(var action in ActionList)
        {
            if(action.TickDown(Time.deltaTime)) {
                DeletionList.Add(action);
            }
        }
        foreach(var actionToDelete in DeletionList)
        {
            ActionList.Remove(actionToDelete);
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