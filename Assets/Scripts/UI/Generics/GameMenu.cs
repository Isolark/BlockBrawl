using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameMenu : MonoBehaviour
{
    public List<GameMenuList> MenuLists;
    public SpriteRenderer MenuCursor;
    protected GameMenuList CurrentMenuList;

    public virtual void SetMenuList(GameMenuList menuList)
    {
        if(CurrentMenuList != null) { CurrentMenuList.Deactivate(); }
        CurrentMenuList = menuList;
        CurrentMenuList.gameObject.SetActive(true);
        CurrentMenuList.Initialize(MenuCursor);
    }
    public virtual void InputMove(Vector2 value)
    {
        if(CurrentMenuList == null) { return; }
        CurrentMenuList.MoveCursor(value);
    }
}