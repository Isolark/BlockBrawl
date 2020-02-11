using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : GameMenu
{
    //public PauseMenuState CurrentState;
    //public TMP_Text MenuTitle;

    public TMP_Text ResumeLabel;
    public TMP_Text RestartLabel;
    public TMP_Text QuitLabel;

    public CanvasGroup AlphaCanvas;
    public Animator AnimCtrl;

    public bool IsOpen;
    public bool IsAnimating;

    private Action ResumeAction;

    override public void Initialize()
    {
        LeftMenuCursor.transform.SetParent(transform);
        RightMenuCursor.transform.SetParent(transform);
        LeftMenuCursor.transform.localScale = Vector3.one;
        RightMenuCursor.transform.localScale = Vector3.one;

        base.Initialize(); 
        SetMenuList(MenuLists[0]);
    }

    public void OpenMenu(Action resumeAction)
    {
        ResumeAction = resumeAction;
        MenuLists[0].gameObject.SetActive(true);

        IsOpen = false;
        IsAnimating = true;

        AnimCtrl.speed = 3;
        AnimCtrl.SetBool("FadeOut", false);
        AnimCtrl.SetTrigger("Play");
    }

    public void CloseMenu()
    {
        IsOpen = true;
        IsAnimating = true;

        Deinitialize();

        AnimCtrl.speed = 3;
        AnimCtrl.SetBool("FadeOut", true);
        AnimCtrl.SetTrigger("Play");
    }

    public void OnFinishAnimation(string clipName)
    {
        if(!IsOpen) 
        {
            IsOpen = true;
            Initialize();
        } 
        else 
        {
            
            IsOpen = false;
            ResumeAction();
        }

        AlphaCanvas.alpha = 1;
        IsAnimating = false;
    }

    public void InputStart()
    {
        if(IsOpen && !IsAnimating) { CloseMenu(); }
    }

    public void InputCancel()
    {
        if(IsOpen && !IsAnimating) { CurrentMenuList.SetCurrentMenuItem(CurrentMenuList.MenuItemList.First().Value); }
    }

    public void InputConfirm()
    {
        if(IsOpen && !IsAnimating) { CurrentMenuList.ConfirmSelection(); }
    }


    //Menu Item Callbacks
    private void SelectResume()
    {
        if(IsOpen && !IsAnimating) { CloseMenu(); }
    }

    private void SelectRestart()
    {
        GameController.GameCtrl.Restart();
    }

    private void SelectQuit()
    {
        GameController.GameCtrl.Quit();
    }
}
