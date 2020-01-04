﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Coordinates multiple effects going off and passing "damage" to opponent
//Source Notes: Approx. size of block. Appear at block and slowly move upwards. After finishing motion, are enclosed by 4 swirling effects
//  If are two (combo & chain), combo appears at normal block position, chain is immediately above that
//  As they are added to the opponent, their damage shifts over to accomodate. Therefore, the spot they go to varies. Will need to communicate to thing displaying it (bar)
public class ComboPop
{
    public SpriteFX PopFX; //Holds logic for the actual combo pop animation. Directs SpriteFXs beneath it
    public SpriteFX PopContainerFX;
    public SpriteFX PopTwirlFX;

    public SpriteFX PopContainerSprite;
    public TMP_Text PopContainerText;

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
        PopFX = SpriteFXPooler.SP.GetPooledObject("ComboPopFX", "SpriteLayer", parent: parentBlock.transform).GetComponent<SpriteFX>();

        PopContainerFX = SpriteFXPooler.SP.GetPooledObject("PopContainerFX", "SpriteLayer", parent: PopFX.transform).GetComponent<SpriteFX>();
        PopContainerSprite = SpriteFXPooler.SP.GetPooledObject("PopSprite", "SpriteLayer", 1, parent: PopContainerFX.transform).GetComponent<SpriteFX>();
        PopContainerSprite.SetSprite((isChain ? "PopBlue" : "PopRed"));

        PopContainerText = TextMeshPooler.TMP.GetPooledObject("ComboPop-TxtConfig", "SpriteLayer", 2, "PopText", parent: PopContainerFX.transform).GetComponent<TMP_Text>();
        PopContainerText.alignment = TextAlignmentOptions.Center;
        PopContainerText.text = isChain ? "x" + Value.ToString() : Value.ToString();

        PopTwirlFX = SpriteFXPooler.SP.GetPooledObject("PopTwirlFX", "SpriteLayer", parent: PopFX.transform).GetComponent<SpriteFX>();
        var PopSpriteLeft = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteLeft", "SpriteLayer", 0, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteTop = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteTop", "SpriteLayer", 0, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteRight = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteRight", "SpriteLayer", 0, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();
        var PopSpriteBot = SpriteFXPooler.SP.GetPooledObject("PopTwirlSpriteBot", "SpriteLayer", 0, parent: PopTwirlFX.transform).GetComponent<SpriteFX>();

        PopSpriteLeft.transform.localPosition = new Vector3(-0.5f, 0, 0);

        PopSpriteLeft.SetSprite("Tomato-Large");
        PopSpriteTop.SetSprite("Tomato-Large");
        PopSpriteRight.SetSprite("Tomato-Large");
        PopSpriteBot.SetSprite("Tomato-Large");

        PopFX.SetAnimator("ComboPopCtrl", true);
        PopFX.StateCallbacks.Add("None", () => Debug.Log("Pop Done"));
    }

    public void Play()
    {
        PopFX.FXAnimCtrl.SetTrigger("Play");
    }
}