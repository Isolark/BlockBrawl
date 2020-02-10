using System.Linq;
using TMPro;
using UnityEngine;

public class GameOverMenu : GameMenu
{
    public TMP_Text RestartLabel;
    public TMP_Text QuitLabel;

    public CanvasGroup AlphaCanvas;
    public Animator AnimCtrl;

    public bool IsOpen;
    public bool IsAnimating;

    override public void Initialize()
    {
        LeftMenuCursor.transform.SetParent(transform);
        RightMenuCursor.transform.SetParent(transform);
        LeftMenuCursor.transform.localScale = Vector3.one;
        RightMenuCursor.transform.localScale = Vector3.one;

        base.Initialize();
        SetMenuList(MenuLists[0]);
    }

    public void OpenMenu()
    {
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
        }

        AlphaCanvas.alpha = 1;
        IsAnimating = false;
    }

    public void InputConfirm()
    {
        if(IsOpen && !IsAnimating) { CurrentMenuList.ConfirmSelection(); }
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