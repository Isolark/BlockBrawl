using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BlockExtensions
{
    public static void Initialize(this Block block, bool isOnBoard = false)
    {
        block.SetStates(isOnBoard);

        var blockSpr = block.GetComponent<SpriteRenderer>();
        var blockIconSpr = block.Icon.GetComponent<SpriteRenderer>();

        blockSpr.sprite = SpriteLibrary.SL.GetSpriteByName("Block-" + block.Type.ToString());
        blockIconSpr.sprite = SpriteLibrary.SL.GetSpriteByName("Middle-" + block.Type.ToString());

        if(!isOnBoard) {
            blockSpr.color = blockIconSpr.color = Color.Lerp(block.BlockSprite.color, Color.black, 0.45f);
        }
    }

    public static void OnEnterBoard(this Block block)
    {
        block.SetStates(true);
        block.GetComponent<SpriteRenderer>().color = block.Icon.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public static void InitStates(this Block block)
    {
        block.IsMoveable = block.IsComboable = true;
        block.IsChainable = false;
        block.IsFalling = false;
    }

    public static void SetStates(this Block block, bool state)
    {
        block.IsComboable = block.IsMoveable = state;
    }

    public static void IncrementType(this Block block, int maxTypes = 5)
    {
        var typeInt = (int)block.Type;

        if(typeInt >= maxTypes) { block.Type = (BlockType)1; }
        else { block.Type = (BlockType)typeInt + 1; }
    }

    public static bool IsMatch(this Block block, BlockType blockType, bool ignoreComboable = false)
    {
        return block.Type == blockType && (block.IsComboable || ignoreComboable);
    }

    //Flashing, states set to false
    public static void StartDestroy(this Block block)
    {
        block.SetStates(false);

        var whiteBlockFX = SpriteFXPooler.SP.GetPooledObject("BlockLayer", parent: block.transform).GetComponent<SpriteFX>();
        whiteBlockFX.Initialize("Block-White", "BlockCtrl");
        whiteBlockFX.FXAnimCtrl.SetTrigger("Play");
    }

    public static void IconDestroy(this Block block)
    {
        block.BlockIconSprite.sprite = null;

        if(block.StoredAction != null) {
            block.StoredAction();
        }
    }

    public static void StartFall(this Block block, bool isChainable, IList<Block> linkedBlocks = null, Action callback = null)
    {
        if(block.IsFalling) { return; }

        block.IsFalling = true;
        block.IsChainable = isChainable;
        block.IsMoveable = block.IsComboable = false;

        block.BoardLoc = new Vector3(block.BoardLoc.x, block.BoardLoc.y - 1, 0);

        var fallDelta = new Vector2(0, -GameController.GC.BlockDist);
        var fallAccel = new Vector2(0, -1.5f);
        var fallMaxVelocity = new Vector2(0, -50f);

        GameController.GC.TransformManager.Add_ManualDeltaPos_Transform(block.gameObject, fallDelta, Vector2.zero, fallAccel, fallMaxVelocity,
            () => block.StepFall(), linkedBlocks.Select(x => x.gameObject).ToList(), callback);
    }

    public static bool StepFall(this Block block)
    {
        var blockList = block.gameObject.GetComponentInParent<BlockContainer>().BlockList;
        var nextBoardLoc = new Vector2(block.BoardLoc.x, block.BoardLoc.y - 1);

        //Remove from previous location
        blockList.Remove(new Vector2(block.BoardLoc.x, block.BoardLoc.y + 1));

        if(blockList.ContainsKey(nextBoardLoc)) { return true; }

        blockList.Add(nextBoardLoc, block);
        block.BoardLoc = nextBoardLoc;

        return false;
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
            GameController.GC.TransformManager.Add_LinearTimePos_Transform(block.gameObject, targetPosition, 0.1f, () => { block.OnFinishMove(); callback(); });
        } else {
            GameController.GC.TransformManager.Add_LinearTimePos_Transform(block.gameObject, targetPosition, 0.1f, block.OnFinishMove);
        }

        block.IsMoveable = block.IsComboable = false;

        if(bumpOrder) {
            block.GetComponent<SpriteRenderer>().sortingOrder = 6;
            block.Icon.GetComponent<SpriteRenderer>().sortingOrder = 7;
        }
    }

    public static void OnFinishMove(this Block block)
    {
        if(block.GetComponent<SpriteRenderer>().sortingOrder >= 6) {
            block.GetComponent<SpriteRenderer>().sortingOrder = 0;
            block.Icon.GetComponent<SpriteRenderer>().sortingOrder = 5; 
        }
    }
}