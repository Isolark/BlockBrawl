using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    //Children
    public GameObject Icon;

    //Vars
    public BlockType Type;
    public BlockStatus Status;
    public SpriteRenderer BlockSprite;
    public SpriteRenderer BlockIconSprite;
    public Animator BlockAnimCtrl;
    public static SpriteLibrary BlockSL;
    public Vector3 BoardLoc;
    public Vector3 PrevBoardLoc;
    public bool IsComboable;
    public bool IsChainable;
    public bool IsMoveable;
    public bool IsMoving;
    public bool IsFalling;
    //public bool IsFallLocked;
    public bool IsDestroying;

    //If all blocks in a garbage block have a fall flag, it can fall
    public bool FallFlag;
    public int FallLockCount;
    public bool IsFallLocked => FallLockCount > 0;
    public bool HasIterated;


    //Action Ref
    public Action StoredAction;
    
    public void OnFinishAnimation(string clipName)
    {
        if(clipName == "Block-FadeOutWhite") {
            BlockSprite.sprite = null;
        } else if (StoredAction != null) {
            StoredAction();
        }
    }
}