using System;
using System.Collections.Generic;
using UnityEngine;

//Parent controller for gameplay. Passes on inputs to relevant game objects
public class BaseController : MonoBehaviour
{
    public GameState GS_Current; 
    public TimedEventManager TimedEventManager;
    public TransformManager TransformManager;

    public delegate void OnFixedUpdateDelegate();
    public event OnFixedUpdateDelegate FixedUpdateDelegate;

    public virtual void AddTimedAction(Action action, float activationTime)
    {
        TimedEventManager.AddTimedAction(action, activationTime);
    }

    protected void FixedUpdate()
    {
        FixedUpdateDelegate();
    }
}