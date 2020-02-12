using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameMenu : MonoBehaviour
{
    public List<MenuList> MenuLists;
    public MenuCursor LeftMenuCursor;
    public MenuCursor RightMenuCursor;
    protected MenuList CurrentMenuList;

    public virtual void Setup()
    {
        foreach(var menuList in MenuLists)
        {
            menuList.Setup();
        }
    }
    public virtual void Initialize()
    {
        if(LeftMenuCursor != null) 
        { 
            LeftMenuCursor.Initialize();
        }
        if(RightMenuCursor != null) 
        { 
            RightMenuCursor.Initialize();
        }
    }
    public virtual void Deinitialize()
    {
        if(LeftMenuCursor != null && LeftMenuCursor.gameObject.activeSelf) 
        { 
            LeftMenuCursor.Deinitialize();
        }
        if(RightMenuCursor != null && RightMenuCursor.gameObject.activeSelf) 
        { 
            RightMenuCursor.Deinitialize(); 
        }
    }
    public virtual void SetMenuList(MenuList menuList, Action callback = null)
    {
        if(CurrentMenuList != null) { CurrentMenuList.Deactivate(); }
        CurrentMenuList = menuList;
        CurrentMenuList.gameObject.SetActive(true);

        if(RightMenuCursor != null) { CurrentMenuList.Initialize(LeftMenuCursor, RightMenuCursor, callback); }
        else { CurrentMenuList.Initialize(LeftMenuCursor, callback); } 
    }
    public virtual void InputMove(Vector2 value)
    {
        if(CurrentMenuList == null) { return; }
        CurrentMenuList.MoveCursor(value);
    }
}