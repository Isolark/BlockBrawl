﻿using UnityEngine;

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
            Destroy(gameObject);
        }     
    }

    // Start is called before the first frame update
    // override protected void Start()
    // {
    //     base.Start();
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