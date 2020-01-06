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

    public static void OnDestroy(this SpriteFX spriteFX) 
    {        
        spriteFX.transform.ResetAllChildrenRecursively();
    }
}