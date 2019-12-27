using UnityEngine;
using UnityEngine.InputSystem;

//Parent controller for gameplay. Passes on inputs to relevant game objects
public class GameController : BaseController, InputActionHub.IPlayerActions
{
    public GameBoard PlayerGameBoard;
    public GameStatsUI GameStatsMenu;
    public float BlockDist;
    public float TimeScale = 1f;
    public float BlockFallVelocity;
    public float BlockFallMaxVelocity;
    public float BlockFallAcceleration;
    public float BlockSwitchSpeed; //Speed at which blocks can be switched
    public float RaiseTimeStopComboMultiplier; //RaiseStopTimer Time += (ComboCount * Multiplier)
    public float RaiseTimeStopChainMultiplier; //RaiseStopTimer Time += (ChainCount * Multiplier)

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

        if(TimeScale != 1) {
            Time.timeScale = TimeScale;
        }
    }

    public void UpdateGameStatMenu(int currentChain)
    {
        GameStatsMenu.CurrentChainValue.text = "x" + currentChain.ToString();

        var maxChain = int.Parse(GameStatsMenu.MaxChainValue.text.Substring(1));
        if(currentChain > maxChain) { GameStatsMenu.MaxChainValue.text = "x" + currentChain.ToString(); }
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
            PlayerGameBoard.OnUpdate();
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
}