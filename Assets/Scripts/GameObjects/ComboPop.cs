using TMPro;
using UnityEngine;

//Coordinates multiple effects going off and passing "damage" to opponent
//Source Notes: Approx. size of block. Appear at block and slowly move upwards. After finishing motion, are enclosed by 4 swirling effects
//  If are two (combo & chain), combo appears at normal block position, chain is immediately above that
//  As they are added to the opponent, their damage shifts over to accomodate. Therefore, the spot they go to varies. Will need to communicate to thing displaying it (bar)
public class ComboPop
{
    public SpriteFX PopHolder; //No logic, just holds everything
    public SpriteFX PopFX; //Holds logic for the actual combo pop animation. Directs SpriteFXs beneath it
    public SpriteFX PopContainerFX;
    public SpriteFX PopTwirlFX;

    public int Value;

    //TODO Section
    public DamageBar TargetDamageBar;

    public void OnFinishAnimation(string clipName)
    {
    }

    //TODO: String for Pass Damage Bar in here
    public ComboPop(int value, bool isChain, Block parentBlock)
    {
        Value = value;    
        PopHolder = SpriteFXPooler.SP.GetPooledObject("PopHolder", "SpriteLayer", parent: parentBlock.transform).GetComponent<SpriteFX>();
        PopFX = SpriteFXPooler.SP.GetPooledObject("ComboPopFX", "SpriteLayer", 0, false, parent: PopHolder.transform).GetComponent<SpriteFX>();

        PopContainerFX = SpriteFXPooler.SP.GetPooledObject("PopContainerFX", "SpriteLayer", 0, false, parent: PopFX.transform).GetComponent<SpriteFX>();
        var popContainerSprite = SpriteFXPooler.SP.GetPooledObject("PopSprite", "SpriteLayer", 1, false, parent: PopContainerFX.transform).GetComponent<SpriteFX>();
        popContainerSprite.SetSprite((isChain ? "PopBlue" : "PopRed"));

        var popContainerText = TextMeshPooler.TMP.GetPooledObject("ComboPop-TxtConfig", "SpriteLayer", 2, "PopText", parent: PopContainerFX.transform).GetComponent<TMP_Text>();
        popContainerText.alignment = TextAlignmentOptions.Center;
        popContainerText.text = isChain ? "x" + Value.ToString() : Value.ToString();

        PopTwirlFX = SpriteFXPooler.SP.GetPooledObject("PopTwirlFX", "SpriteLayer", 0, false, parent: PopFX.transform).GetComponent<SpriteFX>();
        var PopSpriteLeft = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteLeft", "SpriteLayer", 0, false, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteTop = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteTop", "SpriteLayer", 0, false, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteRight = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteRight", "SpriteLayer", 0, false, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteBot = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteBot", "SpriteLayer", 0, false, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();

        PopSpriteLeft.transform.localPosition = new Vector3(-0.5f, 0, 0);

        PopSpriteLeft.SetSprite("Effect-Bun");
        PopSpriteTop.SetSprite("Effect-Bun");
        PopSpriteRight.SetSprite("Effect-Bun");
        PopSpriteBot.SetSprite("Effect-Bun");

        PopFX.SetAnimator("ComboPopCtrl", true);
        PopFX.StateCallbacks.Add("None", () => { PopHolder.OnDestroy(); });
    }

    public void Play()
    {
        PopFX.FXAnimCtrl.SetTrigger("Play");
    }
}