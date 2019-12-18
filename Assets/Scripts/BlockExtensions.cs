using System;
using UnityEngine;

public static class BlockExtensions
{
    public static SpriteLibrary BlockSL;

    public static void Initialize(this Block block, BlockType type, Vector2 location, bool isOnBoard = false)
    {
        block.Type = type;
        block.BoardLoc = location;
        block.SetStates(isOnBoard);

        var blockSpr = block.GetComponent<SpriteRenderer>();
        var blockIconSpr = block.Icon.GetComponent<SpriteRenderer>();

        blockSpr.sprite = BlockSL.GetSpriteByName("Block-" + type.ToString());
        blockIconSpr.sprite = BlockSL.GetSpriteByName("Middle-" + type.ToString());

        if(!isOnBoard) {
            blockSpr.color = blockIconSpr.color = Color.Lerp(block.BlockSprite.color, Color.black, 0.45f);
        }
    }

    public static void OnEnterBoard(this Block block)
    {
        block.SetStates(true);
        block.GetComponent<SpriteRenderer>().color = block.Icon.GetComponent<SpriteRenderer>().color = Color.white;
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

    public static void SetStates(this Block block, bool state)
    {
        block.IsChainable = block.IsComboable = block.IsMoveable = state;
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
