using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameMenu : MonoBehaviour
{
    public List<GameMenuList> MenuLists;
    public SpriteRenderer LeftMenuCursor;
    public SpriteRenderer RightMenuCursor;
    protected GameMenuList CurrentMenuList;

    public virtual void Setup()
    {
        foreach(var menuList in MenuLists)
        {
            menuList.Setup();
        }
    }
    public virtual void Initialize()
    {
        if(LeftMenuCursor != null && !LeftMenuCursor.gameObject.activeSelf) { LeftMenuCursor.gameObject.SetActive(true); }
        if(RightMenuCursor != null && !RightMenuCursor.gameObject.activeSelf) { RightMenuCursor.gameObject.SetActive(true); }
    }
    public virtual void Deinitialize()
    {
        if(LeftMenuCursor != null && LeftMenuCursor.gameObject.activeSelf) { LeftMenuCursor.gameObject.SetActive(false); }
        if(RightMenuCursor != null && RightMenuCursor.gameObject.activeSelf) { RightMenuCursor.gameObject.SetActive(false); }
    }
    public virtual void SetMenuList(GameMenuList menuList)
    {
        if(CurrentMenuList != null) { CurrentMenuList.Deactivate(); }
        CurrentMenuList = menuList;
        CurrentMenuList.gameObject.SetActive(true);

        if(RightMenuCursor != null) { CurrentMenuList.Initialize(LeftMenuCursor, RightMenuCursor); }
        else { CurrentMenuList.Initialize(LeftMenuCursor); } 
    }
    public virtual void InputMove(Vector2 value)
    {
        if(CurrentMenuList == null) { return; }
        CurrentMenuList.MoveCursor(value);
    }
}