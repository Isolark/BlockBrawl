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

    public Animator AnimCtrl;

    public bool IsOpen;
    public bool IsAnimating;

    private Action ResumeAction;

    override protected void Initialize()
    {
        base.Initialize(); 
        SetMenuList(MenuLists[0]);
    }

    public void OpenMenu(Action resumeAction)
    {
        ResumeAction = resumeAction;

        IsOpen = false;
        IsAnimating = true;

        AnimCtrl.SetBool("FadeOut", false);
        AnimCtrl.SetTrigger("Play");
    }

    public void CloseMenu()
    {
        IsOpen = false;
        IsAnimating = true;

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
            Deinitialize();
            ResumeAction(); //TODO: Allow for this to be a passed in Action (if need more than just Resume after fade out)
        }

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
        if(IsOpen && !IsAnimating) { ResumeAction(); }
    }

    private void SelectRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SelectQuit()
    {
        MainController.MC.LoadPrevScene();
    }
}
