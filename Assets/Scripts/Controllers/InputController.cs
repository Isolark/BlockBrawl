using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

//Controller that handles input actions
//Override if need additional conditions met before broadcasting input
public class InputController : MonoBehaviour, InputActionHub.IPlayerActions
{
    public static Vector2 UnitResolution;
    public static int PPU = 100;
    protected InputActionHub InputHub;
    protected Vector2 PrevMoveDir;
    protected float PrevMoveSqrMag;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitializeBinding();
    }

    void InitializeBinding()
    {
        InputHub = new InputActionHub();
        InputHub.Player.SetCallbacks(this);
        InputHub.Player.Enable();

        // var x = new InputBinding();
        // x.GenerateId();
        // x.path = "<Keyboard>/m";

        // var a = new InputAction("confirm");
        // a.AddBinding(x);

        // InputHub.Player.Confirm.ChangeBindingWithId("f7bd21ef-4a6c-4172-8269-c6e6012596c3").To(x);

        // InputHub.Player.SetCallbacks(this);
        // InputHub.Player.Confirm.Disable();
        // InputHub.Player.Confirm.PerformInteractiveRebinding()
        //     .WithControlsExcluding("Mouse")
        //     .OnComplete(CompleteRebinding)
        //     .Start();
    }

    // void CompleteRebinding(InputActionRebindingExtensions.RebindingOperation op)
    // {
    //     Debug.Log(op.selectedControl.device);
    //     Debug.Log(op.selectedControl.displayName);
    //     Debug.Log(op.selectedControl.path);
    
    //     var a = new InputAction();
    //     a.AddBinding("<Keyboard>/n");

    //     InputHub.Player.Confirm.Enable();
    //     op.Dispose();
    // }

    protected virtual void OnDestroy()
    {
        if(InputHub != null) { InputHub.Dispose(); }
    }

    public virtual void OnMove(CallbackContext context)
    {
        var moveDir = context.ReadValue<Vector2>();

        if(MainController.MC.GS_Current == GameState.Active)
        {
            if(!context.performed) 
            { 
                if(moveDir.sqrMagnitude == 0)
                { 
                    PrevMoveDir = Vector2.zero;
                }
                else { return; }
            }
            else if(!CalculateMoveDir(ref moveDir)) { return; }

            BroadcastMessage("InputMove", moveDir, SendMessageOptions.DontRequireReceiver);
        }
        // else if(PrevMoveDir.sqrMagnitude > 0)
        // {
        //     PrevMoveDir = Vector2.zero;
        // }
    }

    protected bool CalculateMoveDir(ref Vector2 moveDir)
    {
        var moveDirSqrMag = moveDir.sqrMagnitude;
        var prevMoveDirSqrMag = PrevMoveDir.sqrMagnitude;

        if(moveDirSqrMag == 0) 
        {
            PrevMoveDir = Vector2.zero;
        }
        else if(PrevMoveDir.sqrMagnitude > 0)
        {
            if(moveDirSqrMag == PrevMoveSqrMag) { moveDir -= PrevMoveDir; }
            else if(moveDirSqrMag < prevMoveDirSqrMag) 
            {
                PrevMoveDir = moveDir;
                return false;
            }
        }

        var tmpPrevMoveDir = PrevMoveDir;

        PrevMoveDir = moveDir;
        moveDir -= tmpPrevMoveDir;

        return true;
    }

    public virtual void OnConfirm(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputConfirm", SendMessageOptions.DontRequireReceiver);
        }
    }

    public virtual void OnCancel(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputCancel", SendMessageOptions.DontRequireReceiver);
        }
    }

    public virtual void OnStart(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputStart", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public virtual void OnTrigger(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputTrigger", context.performed, SendMessageOptions.DontRequireReceiver);
        }
    }
}