﻿using UnityEngine;

public class GameMenuCursor : MonoBehaviour
{
    public SpriteRenderer CursorSprite;
    public Animator CursorAnim;
    public RectTransform RectBounds;
    public float AnimationSpeed;

    private Vector2 SpriteBounds;

    void Awake()
    {
        CursorAnim.speed = AnimationSpeed;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);

        if(SpriteBounds == Vector2.zero) { SpriteBounds = CursorSprite.bounds.size; }
        if(CursorAnim == null) { return; }

        CursorAnim.enabled = true;
        CursorAnim.SetTrigger("Play");
    }
    public void Reinitialize(Transform parentTransform)
    {
        CursorAnim.transform.SetParent(parentTransform);
        CursorAnim.SetTrigger("Play");
    }
    public void Deinitialize()
    {
        CursorAnim.SetTrigger("Stop");
        CursorAnim.enabled = false;
        gameObject.SetActive(false);
    }

    public void SetMenuPosition(GameMenuItem menuItem, bool IsLeftPosition = true)
    {
        gameObject.transform.localPosition = menuItem.transform.localPosition;
        gameObject.TransBySpriteDimensions(menuItem.ItemText.gameObject, new Vector3((IsLeftPosition ? -0.5f : 0.5f), 0.12f, 0));
        gameObject.transform.localPosition += new Vector3((IsLeftPosition ? -SpriteBounds.x : SpriteBounds.x * 0.85f) * RectBounds.localScale.x, 0, 0);
    }
}