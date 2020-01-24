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

    private Action ResumeAction;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Initialize()
    {
        SetMenuList(MenuLists[0]);
    }

    public void OpenMenu(Action resumeAction)
    {
        ResumeAction = resumeAction;

        AnimCtrl.SetBool("FadeOut", false);
        AnimCtrl.SetTrigger("Play");
    }

    public void CloseMenu()
    {
        AnimCtrl.SetBool("FadeOut", true);
        AnimCtrl.SetTrigger("Play");
    }

    public void OnFinishAnimation(string clipName)
    {
        if(!IsOpen) {
            IsOpen = true;
            MenuCursor.gameObject.SetActive(true);
        } else {
            MenuCursor.gameObject.SetActive(false);
        }
    }

    // override public void SetMenuList(GameMenuList menuList)
    // {
    //     base.SetMenuList(menuList);

    //     MenuTitle.text = CurrentMenuList.Title;
    // }

    public void InputStart()
    {
        //Return to Game
        // if(CurrentState == PauseMenuState.PrePauseMenu && StartLabel.gameObject.activeSelf) 
        // {
        //     Initialize();
        // } 
    }

    private void CursorSelect()
    {
    }

    public void InputCancel()
    {
        //Automatically selects Resume OR just Return to Game
        //if(CurrentState == PauseMenuState.PrePauseMenu) { return; }

        // if(CancelAction != null) {
        //     CancelAction();
        //     return;
        // }

        CurrentMenuList.CancelSelection();
    }

    public void InputConfirm()
    {
        //if(CurrentState == PauseMenuState.PrePauseMenu) { return; }

        CurrentMenuList.ConfirmSelection();
    }

    public void InputTrigger()
    {
    }


    //Menu Item Callbacks
    private void ToMain()
    {
        // if(OptionChangedFlag) 
        // {
        //     MainController.MC.SaveOptions();
        //     OptionChangedFlag = false;
        // }
        SetMenuList(MenuLists.First(x => x.name == "PauseMenuList"));
    }
}
