using UnityEngine;
using UnityEngine.InputSystem;

//Parent controller for gameplay. Passes on inputs to relevant game objects
public class GameController : BaseController, InputActionHub.IPlayerActions
{
    public GameBoard PlayerGameBoard;
    public float BlockDist;
    private InputActionHub InputHub;

    public static GameController GC;

    void Awake()
    {
        //Singleton pattern
        if (GC == null) {
            GC = this;
        }
        else if (GC != this) {
            Destroy(gameObject);
        }     
    }

    // Start is called before the first frame update
    void Start()
    {
        GS_Current = GameState.Active;
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

    void CompleteRebinding(InputActionRebindingExtensions.RebindingOperation op)
    {
        Debug.Log(op.selectedControl.device);
        Debug.Log(op.selectedControl.displayName);
        Debug.Log(op.selectedControl.path);


        var a = new InputAction();
        a.AddBinding("<Keyboard>/n");

        InputHub.Player.Confirm.Enable();
        op.Dispose();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        if(GS_Current == GameState.Active)
        {
            PlayerGameBoard.ManUpdate();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Isolate in game layer, not directly to cursor
        if(!context.performed) return;
        if(GS_Current == GameState.Active)
        {
            PlayerGameBoard.SendMessage("OnMove", context.ReadValue<Vector2>());
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if(GS_Current == GameState.Active)
        {
            PlayerGameBoard.SendMessage("OnConfirm");
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
    }

    public void OnTrigger(InputAction.CallbackContext context)
    {
        if(!context.performed && !context.canceled) return;
        if(GS_Current == GameState.Active)
        {
            PlayerGameBoard.SendMessage("OnTrigger", context.performed);
        }
    }


    // public void OnConfirm(InputValue value)
    // {
    //     if(GS_Current == GameState.Active)
    //     {
    //         PlayerCursor.SendMessage("OnConfirm", value);
    //     }
    // }

    // public void OnCancel(InputValue value)
    // {
    //     if(GS_Current == GameState.Active)
    //     {
    //         PlayerCursor.SendMessage("OnCancel", value);
    //     }
    // }
}