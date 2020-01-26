﻿using UnityEngine;

public class MenuController : InputController
{
    public static MenuController MenuCtrl;
    public MainMenu MainMenu;

    void Awake()
    {
        //Singleton pattern
        if (MenuCtrl == null) {
            MenuCtrl = this;
        }
        else if (MenuCtrl != this) {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        MainController.MC.GS_Current = GameState.Active;
        base.Start();

        MainMenu.Setup();

        if(MainController.MC.IsInitialStart)
        {
            MainMenu.Preinitialize();
            MainController.MC.IsInitialStart = false;
        }
        else
        {
            MainMenu.Initialize();
        }

        MainController.MC.PlayMusic("Title");
    }
}