using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : MonoBehaviour 
{
    public MainController MainCtrl;

    private IList<TimedAction> ActionList;
    private IList<TimedAction> StagingList;
    private IList<TimedAction> DeletionList;
    private bool Paused;
    private float PrevTime;

    void Awake()
    {
        ActionList = new List<TimedAction>();
        StagingList = new List<TimedAction>();
        DeletionList = new List<TimedAction>();
        Unpause(true);
    }

    public void Pause()
    {
        if(Paused) { return; }
        MainCtrl.FixedUpdateDelegate -= OnUpdate;
        Paused = true;
    }

    public void Unpause(bool isOverride = false)
    {
        if(!Paused && !isOverride) { return; }
        MainCtrl.FixedUpdateDelegate += OnUpdate;
        Paused = false;
    }

    public void Reset()
    {
        ActionList.Clear();
        StagingList.Clear();
        DeletionList.Clear();
    }

    public TimedAction AddTimedAction(Action action, float activationTime, bool isContinuous = false)
    {
        var timedAction = new TimedAction(action, activationTime, isContinuous);
        StagingList.Add(timedAction);

        return timedAction;
    }

    public void RemoveTimedAction(TimedAction timedAction)
    {
        if(StagingList.Contains(timedAction)) {
            StagingList.Remove(timedAction);
        }
        if(ActionList.Contains(timedAction)) {
            ActionList.Remove(timedAction);
        }
        if(DeletionList.Contains(timedAction)) {
            DeletionList.Remove(timedAction);
        }
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
    private float InitialActivationTime;
    public bool IsActive;
    public bool IsContinuous;
    private Action MyAction;

    public TimedAction(Action myAction, float activationTime, bool isContinuous = false)
    {
        MyAction = myAction;
        InitialActivationTime = ActivationTime = activationTime;
        IsContinuous = isContinuous;
        IsActive = true;
    }

    public bool TickDown(float timeDelta)
    {
        ActivationTime -= timeDelta;

        if(ActivationTime <= 0) {
            MyAction();

            if(IsContinuous) {
                Reset();
                return false;
            }
            return true;
        }
        return false;
    }

    public void SetTime(float activationTime)
    {
        InitialActivationTime = ActivationTime = activationTime;
    }

    public void Reset()
    {
        var startingActivationTime = InitialActivationTime + ActivationTime;
        ActivationTime = startingActivationTime;
    }
}