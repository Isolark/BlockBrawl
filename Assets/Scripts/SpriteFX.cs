using System;
using System.Collections.Generic;
using UnityEngine;

//Sprite that plays through an animation and then deactivates itself (by default)
public class SpriteFX : MonoBehaviour
{
    public bool DestroySelf;
    public Dictionary<string, Action> StateCallbacks;
    public SpriteRenderer FXSprite;
    public Animator FXAnimCtrl;

    void Awake()
    {
        StateCallbacks = new Dictionary<string, Action>();
    }
    public void OnFinishAnimation(string clipName)
    {
        if(StateCallbacks.ContainsKey(clipName)) { StateCallbacks[clipName](); }

        if(DestroySelf && clipName == "None") {
            this.OnDestroy();
        }
    }
}