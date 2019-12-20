using System;
using UnityEngine;

public static class BlockExtensions
{
    public static SpriteLibrary BlockSL;

    public static void Initialize(this Block block, bool isOnBoard = false)
    {
        block.SetStates(isOnBoard);

        var blockSpr = block.GetComponent<SpriteRenderer>();
        var blockIconSpr = block.Icon.GetComponent<SpriteRenderer>();

        blockSpr.sprite = BlockSL.GetSpriteByName("Block-" + block.Type.ToString());
        blockIconSpr.sprite = BlockSL.GetSpriteByName("Middle-" + block.Type.ToString());

        if(!isOnBoard) {
            blockSpr.color = blockIconSpr.color = Color.Lerp(block.BlockSprite.color, Color.black, 0.45f);
        }
    }

    public static void OnEnterBoard(this Block block)
    {
        block.SetStates(true);
        block.GetComponent<SpriteRenderer>().color = block.Icon.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public static void SetStates(this Block block, bool state)
    {
        block.IsChainable = block.IsComboable = block.IsMoveable = state;
    }

    public static bool IsMatch(this Block block, BlockType blockType, bool ignoreComboable = false)
    {
        return block.Type == blockType && (block.IsComboable || ignoreComboable);
    }

    public static void IncrementType(this Block block, int maxTypes = 5)
    {
        var typeInt = (int)block.Type;

        if(typeInt >= maxTypes) { block.Type = (BlockType)1; }
        else { block.Type = (BlockType)typeInt + 1; }
    }

    public static void MoveBoardLoc(this Block block, Vector3 moveVector, bool movePrevBoardLoc = false)
    {
        block.BoardLoc += moveVector;

        if(movePrevBoardLoc) {
            block.PrevBoardLoc += moveVector;
        }
    }

    public static void InstantMove(this Block block, Vector3 moveVector)
    {
        block.MoveBoardLoc(moveVector);

        moveVector.Scale(new Vector3(GameController.GC.BlockDist, GameController.GC.BlockDist, 0));
        block.transform.localPosition += moveVector;

        //Need to be able to handle if raising at the same time 
    }

    public static void Move(this Block block, Vector3 moveVector, bool bumpOrder = false, Action callback = null)
    {
        //Immediately update Array Location
        block.PrevBoardLoc = block.BoardLoc;
        block.MoveBoardLoc(moveVector);

        //Calculate movement
        moveVector.Scale(new Vector3(GameController.GC.BlockDist, GameController.GC.BlockDist, 0));
        var targetPosition = block.transform.localPosition + moveVector;

        if(callback != null) {
            GameController.GC.TransformManager.Add_TimedLinearPos_Transform(block.gameObject, targetPosition, 0.1f, () => { block.OnFinishMove(); callback(); });
        } else {
            GameController.GC.TransformManager.Add_TimedLinearPos_Transform(block.gameObject, targetPosition, 0.1f, block.OnFinishMove);
        }

        block.SetStates(false);

        if(bumpOrder) {
            block.GetComponent<SpriteRenderer>().sortingOrder = 2;
            block.Icon.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }

    public static void OnFinishMove(this Block block)
    {
        block.SetStates(true);
        block.transform.localPosition.Set(block.transform.localPosition.x, block.transform.localPosition.y, 1);

        if(block.GetComponent<SpriteRenderer>().sortingOrder >= 2) {
            block.GetComponent<SpriteRenderer>().sortingOrder = 0;
            block.Icon.GetComponent<SpriteRenderer>().sortingOrder = 1; 
        }
    }
}