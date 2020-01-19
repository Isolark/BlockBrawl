using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

//Controller that handles input actions
public class InputController : BaseController, InputActionHub.IPlayerActions
{
    public static Vector2 UnitResolution;
    public static int PPU = 100;
    protected InputActionHub InputHub;
    protected Vector2 PrevMoveDir;
    protected float PrevMoveSqrMag;


    protected void Awake() 
    {
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        InitializeBinding();
    }

    void InitializeBinding()
    {
        InputHub = new InputActionHub();
        InputHub.Player.SetCallbacks(this);
        InputHub.Player.Enable();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     //NOTE: Realistically, this should be caught somewhere and bubble up here

    //     if(UnitResolution.x != Screen.currentResolution.width * PPU || UnitResolution.y != Screen.currentResolution.height * PPU)
    //     {
    //         SetUnitResolution();
    //     }
    // }

    // Set UnitResolution based on current screen size
    void SetUnitResolution()
    {
        UnitResolution = new Vector2(Screen.currentResolution.width / PPU, Screen.currentResolution.height / PPU);
    }

    public void OnMove(CallbackContext context)
    {
        var moveDir = context.ReadValue<Vector2>();

        if(!context.performed) 
        {
            if(moveDir.sqrMagnitude == 0)
            { 
                PrevMoveDir = Vector2.zero;
                SendMessage("OnMove", moveDir);
            }
            return; 
        }

        if(GS_Current == GameState.Active)
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

            SendMessage("OnMove", moveDir);
        }
        else if(PrevMoveDir.sqrMagnitude > 0)
        {
            PrevMoveDir = Vector2.zero;
        }
    }

    public void OnConfirm(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(GS_Current == GameState.Active)
        {
            SendMessage("OnConfirm");
        }
    }

    public void OnCancel(CallbackContext context)
    {
        if(!context.performed) { return; }
    }

    public void OnStart(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(GS_Current == GameState.Active)
        {
            SendMessage("OnStart", context.performed);
        }
    }
    
    public void OnTrigger(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(GS_Current == GameState.Active)
        {
            SendMessage("OnTrigger", context.performed);
        }
    }
}