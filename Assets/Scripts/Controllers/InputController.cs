﻿using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

//Controller that handles input actions
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
    }

    protected virtual void OnDestroy()
    {
        if(InputHub != null) { InputHub.Dispose(); }
    }

    public void OnMove(CallbackContext context)
    {
        var moveDir = context.ReadValue<Vector2>();

        if(!context.performed) 
        {
            if(moveDir.sqrMagnitude == 0)
            { 
                PrevMoveDir = Vector2.zero;
                BroadcastMessage("InputMove", moveDir, SendMessageOptions.DontRequireReceiver);
            }
            return; 
        }

        if(MainController.MC.GS_Current == GameState.Active)
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
                    return;
                 }
            }

            var tmpPrevMoveDir = PrevMoveDir;

            PrevMoveDir = moveDir;
            moveDir -= tmpPrevMoveDir;

            BroadcastMessage("InputMove", moveDir, SendMessageOptions.DontRequireReceiver);
        }
        else if(PrevMoveDir.sqrMagnitude > 0)
        {
            PrevMoveDir = Vector2.zero;
        }
    }

    public void OnConfirm(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputConfirm", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnCancel(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputCancel", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnStart(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputStart", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public void OnTrigger(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            BroadcastMessage("InputTrigger", SendMessageOptions.DontRequireReceiver);
        }
    }
}