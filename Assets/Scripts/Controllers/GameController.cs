using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

//Parent controller for gameplay. Passes on inputs to relevant game objects
//TODO: OnPause(), trigger an "empty" input for all other bindings (stop player's hold timer, etc...)
public class GameController : InputController
{
    public GameBoard PlayerGameBoard;
    public PauseMenu PauseMenu;
    public GameOverMenu GameOverMenu;
    public float BlockDist;
    public float TimeScale = 1f;
    public float GameTime;
    public float BlockFallVelocity;
    public float BlockSwitchSpeed; //Speed at which blocks can be switched
    
    public float RaiseBaseSpeed; //Starting speed of block raising
    public float RaiseBaseAcceleration; //Starting increase of speed every speed level
    public float RaiseDeltaAcceleration; //Add this to base every 50 levels for current acceleration
    public float RaiseSpeedLevelDelay; //How long until speed level is raised
    public int MaxSpeedLevel;

    public float RaiseTimeStopComboMultiplier; //RaiseStopTimer Time += (ComboCount * Multiplier)
    public float RaiseTimeStopChainMultiplier; //RaiseStopTimer Time += (ChainCount * Multiplier)
    public float RaiseTimeStopBaseComboAmount;

    public bool CanMoveCursor;
    protected bool IsGameStarted;

    public static GameController GameCtrl;

    protected virtual void Awake()
    {
        //Singleton pattern
        if (GameCtrl == null) {
            GameCtrl = this;
        }
        else if (GameCtrl != this) {
            Destroy(GameCtrl);
            GameCtrl = this;
        }     
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        //if(TimeScale != 1) { Time.timeScale = TimeScale; }
        base.Start();

        MainController.MC.GS_Current = GameState.Active;
        
        GameTime = 0;
        
        PauseMenu.Setup();
        GameOverMenu.Setup();
        PauseMenu.gameObject.SetActive(false);
        GameOverMenu.gameObject.SetActive(false);

        IsGameStarted = CanMoveCursor = false;

        MainController.MC.AddTimedAction(() => { PlayerGameBoard.Initialize(5, MaxSpeedLevel, RaiseBaseSpeed, RaiseBaseAcceleration); }, 0.4f);
    }

    public void StartGame()
    {
        IsGameStarted = true;
        PlayerGameBoard.StartGame();
    }

    public virtual void LoseGame()
    {
        PlayerGameBoard.EndGame(false);

        MainController.MC.Pause();

        GameOverMenu.gameObject.SetActive(true);
        GameOverMenu.OpenMenu();
    }

    public virtual void WinGame()
    {
        PlayerGameBoard.EndGame(true);
    }

    public virtual void OpenGameOverMenu()
    {
        MainController.MC.Pause();

        GameOverMenu.gameObject.SetActive(true);
        GameOverMenu.OpenMenu();
    }

    public virtual void OpenVictoryMenu()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(MainController.MC.GS_Current == GameState.Active)
        {
            if(IsGameStarted)
            {
                GameTime += Time.deltaTime;
                //ScoreModeMenu.SetGameTime(GameTime);
                
                PlayerGameBoard.OnUpdate();
            }
        }
    }

    override public void OnMove(CallbackContext context)
    {
        var moveDir = context.ReadValue<Vector2>();

        if(!context.performed) 
        {
            if(moveDir.sqrMagnitude == 0)
            { 
                PrevMoveDir = Vector2.zero;
            }
            else { return; }
        }
        else if(!CalculateMoveDir(ref moveDir)) { return; }

        if(MainController.MC.GS_Current == GameState.Active && (IsGameStarted || CanMoveCursor))
        {
            PlayerGameBoard.InputMove(moveDir);
        }
        else if(MainController.MC.GS_Current == GameState.MenuOpen)
        {
            if(PauseMenu.gameObject.activeSelf) { PauseMenu.InputMove(moveDir); }
            else { GameOverMenu.InputMove(moveDir); }
        }
    }
    override public void OnStart(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active && (IsGameStarted || CanMoveCursor))
        {
            Pause();
        }
        else
        {
            if(PauseMenu.gameObject.activeSelf) { PauseMenu.InputStart(); }
        }
    }

    override public void OnConfirm(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active && IsGameStarted)
        {
            PlayerGameBoard.InputConfirm();
        }
        else if(MainController.MC.GS_Current == GameState.MenuOpen)
        {
            if(PauseMenu.gameObject.activeSelf) { PauseMenu.InputConfirm(); }
            else { GameOverMenu.InputConfirm(); }
        }
    }

    override public void OnCancel(CallbackContext context)
    {
        if(!context.performed) { return; }
        if(MainController.MC.GS_Current == GameState.Active && IsGameStarted)
        {
        }
        else if(MainController.MC.GS_Current == GameState.MenuOpen)
        {
            if(PauseMenu.gameObject.activeSelf) { PauseMenu.InputCancel(); }
        }
    }

    public void Pause()
    {
        if(MainController.MC.GS_Current == GameState.Active && (IsGameStarted || CanMoveCursor))
        {
            //TODO: SFX
            //Clears "Hold" on input receivers
            PlayerGameBoard.InputMove(Vector2.zero);
            PlayerGameBoard.Pause();

            MainController.MC.Pause();

            PauseMenu.gameObject.SetActive(true);
            PauseMenu.OpenMenu(Unpause);
        }
    }

    public void Unpause()
    {
        //Clears "Hold" on input receivers
        PlayerGameBoard.Unpause();
        PauseMenu.InputMove(Vector2.zero);

        MainController.MC.Unpause();
        PauseMenu.gameObject.SetActive(false);
    }

    public void Restart()
    {
        PlayerGameBoard.ResetAnimations();
        MainController.MC.GoToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        MainController.MC.LoadPrevScene();
    }
}