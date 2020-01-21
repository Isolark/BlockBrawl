using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuList : DirectionInputReceiver
{
    public Dictionary<Vector2, GameMenuItem> MenuItemList;
    public GameMenuItem CurrentMenuItem;
    public string Title;
    public string CancelMessage;
    public bool CanHoldX, CanHoldY; //We enabled, can "Hold" direction inputs
    public bool WillResetPosition;

    private int MinLocX, MaxLocX, MinLocY, MaxLocY;
    private SpriteRenderer MenuCursor;
    
    
    void Start()
    {
        MenuItemList = new Dictionary<Vector2, GameMenuItem>();

        foreach(var menuItem in GetComponentsInChildren<GameMenuItem>())
        {
            MenuItemList.Add(menuItem.ListLoc, menuItem);

            if(menuItem.ListLoc.x <= MinLocX) { MinLocX = (int)menuItem.ListLoc.x; }
            if(menuItem.ListLoc.x >= MaxLocX) { MaxLocX = (int)menuItem.ListLoc.x; }
            if(menuItem.ListLoc.y <= MinLocY) { MinLocY = (int)menuItem.ListLoc.y; }
            if(menuItem.ListLoc.y >= MaxLocY) { MaxLocY = (int)menuItem.ListLoc.y; }
        }

        gameObject.SetActive(false);
    }

    public void Initialize(SpriteRenderer menuCursor)
    {
        MenuCursor = menuCursor;
        if(!MenuCursor.gameObject.activeSelf) { MenuCursor.gameObject.SetActive(true); }
        MenuCursor.transform.SetParent(MenuItemList.First().Value.transform.parent);

        if(WillResetPosition || CurrentMenuItem == null) { CurrentMenuItem = MenuItemList[new Vector2(MinLocX, MaxLocY)]; }

        ResetHold();
        SetCurrentMenuItem(CurrentMenuItem);
    }

    public void Deactivate()
    {
        ResetHold();
        gameObject.SetActive(false); 
    }

    private void IncrementSlider(Vector2 direction)
    {
        var stepValue = CurrentMenuItem.LinkedSlider.GetComponent<EXSlider>().StepValue;
        CurrentMenuItem.LinkedSlider.value += stepValue * direction.x;
        if(CurrentMenuItem.LinkedSlider.value < 0) { CurrentMenuItem.LinkedSlider.value = 0; }
        ConfirmSelection();
    }

    public void MoveCursor(Vector2 value)
    {
        ResetHold();

        if(value == Vector2.zero) { return; }

        var breakIndex = 200;
        Vector2 nextListLoc = CurrentMenuItem.ListLoc + value;

        for(;;)
        {
            breakIndex--;
            if(breakIndex <= 0) { 
                Debug.LogError("Break Index Hit");
                break; 
            }

            if(value == Vector2.up && nextListLoc.y > MaxLocY) { nextListLoc.y = MinLocY; }
            else if(value == Vector2.down && nextListLoc.y < MinLocY) { nextListLoc.y = MaxLocY; }
            else if(value == Vector2.left) 
            { 
                if(CurrentMenuItem.LinkedSlider != null)
                {
                    OnMove(value, IncrementSlider);
                    break;
                }
                else if(nextListLoc.x < MinLocX) { nextListLoc.x = MaxLocX; }
            }
            else if(value == Vector2.right) 
            {
                if(CurrentMenuItem.LinkedSlider != null)
                {
                    OnMove(value, IncrementSlider);
                    break;
                }
                else if(nextListLoc.x > MaxLocX) { nextListLoc.x = MinLocX; }
            }
            

            GameMenuItem nextMenuItem;
            if(!MenuItemList.TryGetValue(nextListLoc, out nextMenuItem) || !nextMenuItem.IsSelectable) 
            { 
                nextListLoc += value;
                continue; 
            }
            SetCurrentMenuItem(nextMenuItem);

            break;
        }
    }

    public void CancelSelection()
    {
        if(!string.IsNullOrWhiteSpace(CancelMessage)) 
        {
            SendMessageUpwards(CancelMessage, SendMessageOptions.DontRequireReceiver); 
        }
    }

    public void ConfirmSelection()
    {
        if(!CurrentMenuItem.IsSelectable)
        {
            //TODO: Invalid SoundFX
            return;
        }

        //TODO: Valid SoundFX
        SendMessageUpwards(CurrentMenuItem.ActionName, SendMessageOptions.DontRequireReceiver);
    }

    public void SetCurrentMenuItem(GameMenuItem menuItem)
    {
        CurrentMenuItem = menuItem;
        MenuCursor.transform.localPosition = CurrentMenuItem.transform.localPosition;
        MenuCursor.gameObject.TransBySpriteDimensions(CurrentMenuItem.ItemText.gameObject, new Vector3(-0.5f, 0.28f, 0));
        MenuCursor.gameObject.TransBySpriteDimensions(new Vector3(-2f * 18, 0, 0));
    }
}