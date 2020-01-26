using System.Collections.Generic;
using UnityEngine;

public static class SpriteFXExtensions
{
    public static void Initialize(this SpriteFX spriteFX, string spriteName, string animCtrlName, bool isEnabled = false)
    {
        spriteFX.SetSprite(spriteName);
        spriteFX.SetAnimator(animCtrlName, isEnabled);
    }

    public static void SetSprite(this SpriteFX spriteFX, string spriteName)
    {
        spriteFX.FXSprite.sprite = SpriteLibrary.SL.GetSpriteByName(spriteName);
    }

    public static void SetAnimator(this SpriteFX spriteFX, string animCtrlName, bool isEnabled = false)
    {
        spriteFX.FXAnimCtrl.runtimeAnimatorController = AnimatorLibrary.AL.GetAnimatorByName(animCtrlName);
        spriteFX.FXAnimCtrl.enabled = isEnabled;
    }

    public static void Reset(this SpriteFX spriteFX)
    {
        spriteFX.StateCallbacks.Clear();
        spriteFX.FXAnimCtrl.runtimeAnimatorController = null;
        spriteFX.FXAnimCtrl.enabled = false;
        spriteFX.FXSprite.color = Color.white;
        spriteFX.FXSprite.sprite = null;

        spriteFX.transform.ResetTransform(true);
    }

    public static void OnDestroy(this SpriteFX spriteFX) 
    {     
        if(spriteFX.gameObject.activeSelf) { SpriteFXPooler.SP.RepoolSpriteFX(spriteFX); } 
        //spriteFX.transform.ResetAllChildrenRecursively();
    }
}