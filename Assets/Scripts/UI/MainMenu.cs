using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MainMenuState
{
    PreMainMenu = 0,
    OnMainMenu = 1,
    InSinglePlayerMenu = 2,
    InMultiPlayerMenu = 3,
    InOptionsMenu = 4
}

public class MainMenu : MonoBehaviour
{
    public MainMenuState CurrentState;
    public CanvasGroup MenuCanvas;
    public Sprite MenuCursor;

    public TMP_Text StartLabel;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = MainMenuState.PreMainMenu;
    }

    public void OnConfirm()
    {
        //if(CurrentState == MainMenuState.)
    }

    public void OnStart(bool performed)
    {
        if(CurrentState == MainMenuState.PreMainMenu && StartLabel.gameObject.activeSelf) {
            //TODO: Sound FX
            StartLabel.gameObject.SetActive(false);
            MenuCanvas.gameObject.SetActive(true);
            CurrentState = MainMenuState.OnMainMenu;
        } 
    }

    private void CursorSelect()
    {

    }

    public void OnTrigger(bool performed)
    {
        //BlockContainer.OnTriggerRaise(performed);
    }

    public void OnMove(Vector2 value)
    {
        //Cursor.OnMove(value);
    }
}
