using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BlockExtensions
{
    public static void Initialize(this Block block, bool isOnBoard = false)
    {
        block.SetStates(isOnBoard);
        block.StoredAction = null;

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
        block.InitStates();
        block.GetComponent<SpriteRenderer>().color = block.Icon.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public static void InitStates(this Block block)
    {
        block.IsMoveable = block.IsComboable = true;
        block.IsMoving = false;
        block.IsChainable = false;
        block.IsFalling = false;
        block.FallFlag = false;
        block.FallLockCount = 0;
        block.IsDestroying = false;
        
        block.Status = BlockStatus.Normal;
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

    public static bool CanFall(this Block block)
    {
        //TODO: Add Garbage Block logic for checking if anything below any of the blocks
        return !block.IsDestroying;
    }

    public static bool IsMatch(this Block block, Block blockToMatch, bool ignoreState = false)
    {
        return block.GetInstanceID() != blockToMatch.GetInstanceID() && 
            block.Type == blockToMatch.Type && ((block.IsComboable && !block.IsDestroying && !block.IsFalling) || ignoreState);
    }

    //Destroy and display combo and or chain. Need to create the ComboPops that will 
    public static void StartDestroy(this Block block, int comboCount, int chainCount)
    {
        block.StartDestroy();
    }

    //Flashing, states set to false
    public static void StartDestroy(this Block block)
    {
        block.SetStates(false);
        block.IsDestroying = true;

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

    //If Block was locked in expectation of falling, allow to move (if not destroying)
    public static void RemoveFallLock(this Block block)
    {
        if(block.IsDestroying) { return; }

        block.FallLockCount--;
        if(block.FallLockCount < 0) { block.FallLockCount = 0; }

        block.IsMoveable = !block.IsMoving;
    }

    public static void NullSwapMove(this Block block, IDictionary<Vector2, Block> blockList, Block nullBlock, Vector2 nextLoc)
    {
        blockList[block.BoardLoc] = nullBlock;
        block.PrevBoardLoc = block.BoardLoc;
        block.BoardLoc = nextLoc;
        blockList[nextLoc] = block;
    }

    public static void StartFall(this Block block, bool isChainable, IList<Block> linkedBlocks = null, Action callback = null)
    {
        if(block.IsFalling || block.IsDestroying ) { return; }

        var container = block.gameObject.GetComponentInParent<BlockContainer>();
        var blockList = container.BlockList;
        var nullBlock = container.NullBlock;

        block.IsFalling = true;
        block.IsChainable = isChainable;
        block.IsMoveable = block.IsComboable = false;
        block.FallLockCount = 0;

        var nextLoc = new Vector3(block.BoardLoc.x, block.BoardLoc.y - 1, 0);
        
        block.NullSwapMove(blockList, nullBlock, nextLoc);

        if(linkedBlocks != null) 
        {
            foreach(var linkedBlock in linkedBlocks)
            {
                if(!linkedBlock.IsChainable) { linkedBlock.IsChainable = isChainable; }

                linkedBlock.IsFalling = true;
                linkedBlock.IsMoveable = linkedBlock.IsComboable = false;
                linkedBlock.FallLockCount = 0;

                var nextLinkedLoc = new Vector3(linkedBlock.BoardLoc.x, linkedBlock.BoardLoc.y - 1, 0);
                linkedBlock.NullSwapMove(blockList, nullBlock, nextLinkedLoc);
            }
        }

        var fallDelta = new Vector2(0, -GameController.GC.BlockDist);
        var fallAccel = new Vector2(0, -GameController.GC.BlockFallAcceleration);
        var fallVelocity = new Vector2(0, -GameController.GC.BlockFallVelocity);
        var fallMaxVelocity = new Vector2(0, -GameController.GC.BlockFallMaxVelocity);

        var linkedObjs = linkedBlocks != null ? linkedBlocks.Select(x => x.gameObject).ToList() : null;

        GameController.GC.TransformManager.Add_ManualDeltaPos_Transform(block.gameObject, fallDelta, fallVelocity, fallMaxVelocity, fallAccel,
            () => block.StepFall(linkedBlocks), linkedObjs, callback);
    }

    public static bool StepFall(this Block block, IList<Block> linkedBlocks = null)
    {
        var container = block.gameObject.GetComponentInParent<BlockContainer>();
        var blockList = container.BlockList;
        var nullBlock = container.NullBlock;

        var nextBoardLoc = new Vector2(block.BoardLoc.x, block.BoardLoc.y - 1);

        //Remove from previous location (if not already filled)
        if(blockList.ContainsKey(block.PrevBoardLoc) && 
            (blockList[block.PrevBoardLoc].GetInstanceID() == nullBlock.GetInstanceID() ||
            blockList[block.PrevBoardLoc].GetInstanceID() == block.GetInstanceID())) 
        {
            blockList.Remove(block.PrevBoardLoc);
        }

        if(linkedBlocks != null) 
        {
            foreach(var linkedBlock in linkedBlocks) 
            {
                if(blockList.ContainsKey(linkedBlock.PrevBoardLoc) && 
                    (blockList[linkedBlock.PrevBoardLoc].GetInstanceID() == nullBlock.GetInstanceID() ||
                    blockList[linkedBlock.PrevBoardLoc].GetInstanceID() == linkedBlock.GetInstanceID())) 
                {
                    blockList.Remove(linkedBlock.PrevBoardLoc);
                }
            }
        }

        if(blockList.ContainsKey(nextBoardLoc)) 
        { 
            var blockBelow = blockList[nextBoardLoc];

            if(blockBelow == nullBlock) 
            {
                Block leftBlock;
                var isLeftBlock = blockList.TryGetValue(nextBoardLoc + Vector2.left, out leftBlock);

                if(isLeftBlock && leftBlock.IsMoving && 
                    (leftBlock.PrevBoardLoc.x == nextBoardLoc.x && leftBlock.PrevBoardLoc.y == nextBoardLoc.y)) { return true; }

                Block rightBlock;
                var isRightBlock = blockList.TryGetValue(nextBoardLoc + Vector2.right, out rightBlock);

                if(isRightBlock && rightBlock.IsMoving && 
                    (rightBlock.PrevBoardLoc.x == nextBoardLoc.x && rightBlock.PrevBoardLoc.y == nextBoardLoc.y)) { return true; }

                blockList.Remove(nextBoardLoc);
            }
            else
            {
                return true;
            }
        }

        block.NullSwapMove(blockList, nullBlock, nextBoardLoc);

        if(linkedBlocks != null) {
            foreach(var linkedBlock in linkedBlocks) {
                var nextLinkedBlockLoc = new Vector2(linkedBlock.BoardLoc.x, linkedBlock.BoardLoc.y - 1);

                if(blockList.ContainsKey(nextLinkedBlockLoc)) {
                    blockList.Remove(nextLinkedBlockLoc);
                }

                linkedBlock.NullSwapMove(blockList, nullBlock, nextLinkedBlockLoc);
            }
        }

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
        var switchSpeed = GameController.GC.BlockSwitchSpeed;

        if(callback != null) {
            GameController.GC.TransformManager.Add_LinearTimePos_Transform(block.gameObject, targetPosition, switchSpeed, () => { block.OnFinishMove(); callback(); });
        } else {
            GameController.GC.TransformManager.Add_LinearTimePos_Transform(block.gameObject, targetPosition, switchSpeed, block.OnFinishMove);
        }

        block.IsMoveable = block.IsComboable = false;
        block.IsMoving = true;

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

        block.IsMoving = false;
    }
}