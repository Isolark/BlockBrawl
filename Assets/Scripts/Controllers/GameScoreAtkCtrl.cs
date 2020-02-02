using UnityEngine;

public class GameScoreAtkCtrl : GameController
{
    public ScoreModeMenu ScoreModeMenu;

    public static GameScoreAtkCtrl GameSA_Ctrl;

    override protected void Awake()
    {
        base.Awake();

        //Singleton pattern
        if (GameSA_Ctrl == null) {
            GameSA_Ctrl = this;
        }
        else if (GameSA_Ctrl != this) {
            Destroy(GameSA_Ctrl);
            GameSA_Ctrl = this;
        }     
    }

    // Start is called before the first frame update
    // override protected void Start()
    // {
    //     //GameBoard

    //     //if(TimeScale != 1) { Time.timeScale = TimeScale; }
    //     base.Start();

    //     MainController.MC.GS_Current = GameState.Active;
        
    //     GameTime = 0;
        
    //     PauseMenu.Setup();
    //     PauseMenu.gameObject.SetActive(false);

    //     IsGameStarted = CanMoveCursor = false;

    //     MainController.MC.AddTimedAction(() => { PlayerGameBoard.Initialize(MaxSpeedLevel, RaiseBaseSpeed, RaiseBaseAcceleration); }, 0.4f);
    // }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        if(MainController.MC.GS_Current == GameState.Active)
        {
            if(IsGameStarted)
            {
                ScoreModeMenu.SetGameTime(GameTime);
            }
        }
    }
}