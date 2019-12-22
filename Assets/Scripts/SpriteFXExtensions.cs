using System.Collections.Generic;
using UnityEngine;

public static class SpriteFXExtensions
{
    public static void Initialize(this SpriteFX spriteFX, string spriteName, string animName)
    {
        spriteFX.FXSprite.sprite = SpriteLibrary.SL.GetSpriteByName(spriteName);
        spriteFX.FXAnimCtrl.runtimeAnimatorController = AnimatorLibrary.AL.GetAnimatorByName(animName);
        spriteFX.FXAnimCtrl.enabled = true;
    }

    public static void Initialize(this SpriteFX spriteFX, string animName, IList<string> triggerList)
    {

    }

    public static void OnDestroy(this SpriteFX spriteFX) 
    {
        spriteFX.FXAnimCtrl.enabled = false;
        spriteFX.FXSprite.color = Color.white;
        spriteFX.gameObject.SetActive(false);
    }
}