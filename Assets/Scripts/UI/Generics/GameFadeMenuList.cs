using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameFadeMenuList : GameMenuList
{
    public CanvasGroup AlphaCanvas;

    public Vector3 FadeInVector;
    public float FadeInAlpha;
    public float FadeInDuration;
    public float FadeInStagger;

    public Vector3 FadeOutVector;
    public float FadeOutAlpha;
    public float FadeOutDuration;
    public float FadeOutStagger;

    public float CursorFadeDuration;

    private bool InTransition;

    override public void Setup()
    {
        base.Setup();        
        foreach(var menuItem in MenuItemList.Values)
        {
            menuItem.transform.localPosition -= FadeInVector;
            menuItem.InitialPos = menuItem.transform.localPosition;
        }
        ResetItems();
    }

    private void ResetItems()
    {
        foreach(var menuItem in MenuItemList.Values)
        {
            menuItem.transform.localPosition = menuItem.InitialPos;
        }
        AlphaCanvas.alpha = FadeInAlpha;
        InTransition = false;
    }

    override public void Initialize(GameMenuCursor leftMenuCursor, GameMenuCursor rightMenuCursor)
    {
        base.Initialize(leftMenuCursor, rightMenuCursor); 

        leftMenuCursor.CursorImage.CrossFadeAlpha(0, CursorFadeDuration, false);
        rightMenuCursor.CursorImage.CrossFadeAlpha(0, CursorFadeDuration, false);

        Transition(() => { 
            leftMenuCursor.CursorImage.CrossFadeAlpha(1, CursorFadeDuration, false);
            rightMenuCursor.CursorImage.CrossFadeAlpha(1, CursorFadeDuration, false);
            SetDefaultMenuItem();
        }, true);
    }

    override public void Initialize(GameMenuCursor menuCursor)
    {
        if(InTransition) {
            LeftMenuCursor = menuCursor;
            LeftMenuCursor.Reinitialize(MenuItemList.First().Value.transform.parent);
            
            return;
        }

        base.Initialize(menuCursor); 
        menuCursor.CursorImage.CrossFadeAlpha(0, CursorFadeDuration, false);

        Transition(() => { 
            menuCursor.CursorImage.CrossFadeAlpha(1, CursorFadeDuration, false);
            SetDefaultMenuItem();
        }, true);
    }

    override public void Deactivate()
    {
        Transition(() => {
            ResetItems();
            base.Deactivate();
        }, false);
    }

    private void SetDefaultMenuItem()
    {
        InTransition = false;
        if(WillResetPosition || CurrentMenuItem == null) { CurrentMenuItem = MenuItemList[new Vector2(MinLocX, MaxLocY)]; }

        ResetHold();
        SetCurrentMenuItem(CurrentMenuItem);
    }

    private void Transition(Action callback, bool isFadeIn)
    {
        InTransition = true;
        var stagger = 0f;

        var fadeVector = isFadeIn ? FadeInVector : FadeOutVector;
        var fadeDuration = isFadeIn ? FadeInDuration : FadeOutDuration;
        var fadeStagger = isFadeIn ? FadeInStagger : FadeOutStagger;

        for(var i = 0; i < MenuItemList.Values.Count; i++)
        {
            var menuItem = MenuItemList.Values.ElementAt(i);

            Action finalCallback = null;
            if(i == MenuItemList.Values.Count - 1) { finalCallback = callback; }

            var nextPos = menuItem.transform.localPosition + fadeVector;

            if(stagger == 0) {
                MainController.MC.TransformManager.Add_LinearTimePos_Transform(menuItem.gameObject, nextPos, fadeDuration, finalCallback);
            }
            else
            {
                MainController.MC.AddTimedAction(() => { 
                    MainController.MC.TransformManager.Add_LinearTimePos_Transform(menuItem.gameObject, nextPos, fadeDuration, finalCallback);}, stagger);
            }
            stagger += fadeStagger;
        }

        if((isFadeIn && AlphaCanvas.alpha != 1) || (!isFadeIn && AlphaCanvas.alpha > FadeOutAlpha)) { 
            StartCoroutine(FadeCanvas(2, isFadeIn)); 
        }
    }

    private IEnumerator FadeCanvas(float fadeMultiplier, bool isFadeIn)
    {
        while((isFadeIn && AlphaCanvas.alpha < 1) || (!isFadeIn && AlphaCanvas.alpha > FadeOutAlpha))
        {
            var nextAlpha = AlphaCanvas.alpha + Time.deltaTime * fadeMultiplier * (isFadeIn ? 1 : -1);
            AlphaCanvas.alpha = nextAlpha;
            yield return null;
        }
        AlphaCanvas.alpha = isFadeIn ? 1 : FadeInAlpha;
    }

    override public void MoveCursor(Vector2 value)
    {
        if(InTransition) { return; }
        base.MoveCursor(value);
    }

    override public void CancelSelection()
    {
        if(InTransition) { return; }
        base.CancelSelection();
    }

    override public void ConfirmSelection()
    {
        if(InTransition) { return; }
        base.ConfirmSelection();
    }
}