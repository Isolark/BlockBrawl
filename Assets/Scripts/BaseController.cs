using System;
using UnityEngine;

//Parent controller for gameplay. Passes on inputs to relevant game objects
public class BaseController : MonoBehaviour
{
    public GameState GS_Current; 
    public TimedEventManager TimedEventManager;
    public TransformManager TransformManager;

    public delegate void OnFixedUpdateDelegate();
    public event OnFixedUpdateDelegate FixedUpdateDelegate;
    public delegate void OnUpdateDelegate();
    public event OnUpdateDelegate UpdateDelegate;

    public virtual void AddTimedAction(Action action, float activationTime)
    {
        TimedEventManager.AddTimedAction(action, activationTime);
    }

    protected virtual void FixedUpdate()
    {
        //FixedUpdateDelegate();
    }

    protected virtual void Update()
    {
        UpdateDelegate();
    }
}