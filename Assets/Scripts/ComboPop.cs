using System.Collections.Generic;
using UnityEngine;

//Coordinates multiple effects going off and passing "damage" to opponent
//Source Notes: Approx. size of block. Appear at block and slowly move upwards. After finishing motion, are enclosed by 4 swirling effects
//  If are two (combo & chain), combo appears at normal block position, chain is immediately above that
//  As they are added to the opponent, their damage shifts over to accomodate. Therefore, the spot they go to varies. Will need to communicate to thing displaying it (bar)
public class ComboPop : MonoBehaviour
{
    public Vector2 DestroySelf;
    public SpriteFX PopSprite;
    public List<SpriteFX> CircleSprites;

    public void OnFinishAnimation(string clipName)
    {
    }
}