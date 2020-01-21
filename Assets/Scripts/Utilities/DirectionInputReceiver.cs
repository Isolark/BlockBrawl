using System;
using UnityEngine;

//Allow for more tricky input handling for directional inputs
public abstract class DirectionInputReceiver : MonoBehaviour
{
    public float HoldInitialDelay; //Delay before triggering multiple times
    public float HoldStepDelay; //Delay between "Hold" actions
    public TimedAction HoldingTA; //Timer for performing a "Hold" action
    public bool IsHoldMoving; //Have triggered a "Hold" action
    public Vector2 HoldingDir; //The "stored" direction
    protected void ResetHold()
    {
        IsHoldMoving = false;
        HoldingDir = Vector2.zero;
        MainController.MC.RemoveTimedAction(HoldingTA);
        HoldingTA = null;
    }
    public virtual void OnMove(Vector2 value, Action<Vector2> moveAction)
    {
        if(IsHoldMoving) { IsHoldMoving = false; }

        if(value == Vector2.zero)
        {
            ResetHold();
        }
        else if(HoldingTA == null)
        {
            HoldingTA = MainController.MC.AddTimedAction(() => { StartHoldMovement(moveAction); }, HoldInitialDelay, true);
        }
        else
        {
            HoldingTA.SetTime(HoldInitialDelay);
        }

        HoldingDir = value;

        moveAction(value);
    }

    private void StartHoldMovement(Action<Vector2> holdAction)
    {
        if(!IsHoldMoving)
        {
            IsHoldMoving = true;
            HoldingTA.SetTime(HoldStepDelay);
        }

        holdAction(HoldingDir);
    }
    void Destroy()
    {
        if(HoldingTA != null) { ResetHold(); }
    }
}