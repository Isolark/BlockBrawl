using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

//Parent controller for gameplay. Passes on inputs to relevant game objects
//TODO: OnPause(), trigger an "empty" input for all other bindings (stop player's hold timer, etc...)
public class GameController : InputController
{
    public GameBoard PlayerGameBoard;
    public GameStatsUI GameStatsMenu;
    public float BlockDist;
    public float TimeScale = 1f;
    public float GameTime;
    public float BlockFallVelocity;
    public float BlockFallMaxVelocity;
    public float BlockFallAcceleration;
    public float BlockSwitchSpeed; //Speed at which blocks can be switched
    public float RaiseTimeStopComboMultiplier; //RaiseStopTimer Time += (ComboCount * Multiplier)
    public float RaiseTimeStopChainMultiplier; //RaiseStopTimer Time += (ChainCount * Multiplier)
    public float RaiseTimeStopBaseComboAmount;

    public static GameController GameCtrl;

    void Awake()
    {
        //Singleton pattern
        if (GameCtrl == null) {
            GameCtrl = this;
        }
        else if (GameCtrl != this) {
            Destroy(gameObject);
        }     
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        MainController.MC.GS_Current = GameState.Active;

        GameTime = 0;
        //InitializeBinding();

        if(TimeScale != 1) { Time.timeScale = TimeScale; }
    }

    public void UpdateGameStatMenu(int currentChain)
    {
        GameStatsMenu.SetCurrentChain(currentChain);

        var maxChain = int.Parse(GameStatsMenu.MaxChainValue.text.Substring(1));
        if(currentChain > maxChain) { GameStatsMenu.SetMaxChain(currentChain); }
    }

    // Update is called once per frame
    void Update()
    {
        if(MainController.MC.GS_Current == GameState.Active)
        {
            GameTime += Time.deltaTime;
            GameStatsMenu.SetGameTime(GameTime);
            
            PlayerGameBoard.OnUpdate();
        }
    }

    public void OnMove(CallbackContext context)
    {
        var moveDir = context.ReadValue<Vector2>();

        if(!context.performed) 
        {
            if(moveDir.sqrMagnitude == 0)
            { 
                PrevMoveDir = Vector2.zero; 
                PlayerGameBoard.SendMessage("OnMove", moveDir);
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

            PlayerGameBoard.SendMessage("OnMove", moveDir);
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
            PlayerGameBoard.SendMessage("OnConfirm");
        }
    }

    public void OnCancel(CallbackContext context)
    {
        if(!context.performed) { return; }
    }

    public void OnStart(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            SendMessage("OnStart", context.performed);
        }
    }

    public void OnTrigger(CallbackContext context)
    {
        if(!context.performed && !context.canceled) { return; }
        if(MainController.MC.GS_Current == GameState.Active)
        {
            PlayerGameBoard.SendMessage("OnTrigger", context.performed);
        }
    }
}