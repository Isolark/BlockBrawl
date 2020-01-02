using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    public GameCursor Cursor;
    public Vector2 BoardSize;
    public int MaxTypes;
    public int StartingHeight;
    public float IconDestroyDelay; //Delay between icons being destroyed; Reduce for faster Gameplay
    public float ChainLinkDelay; //Delay between starting/adding to a chain VS starting a new chain (should allow for 2 diff rows being diff chains)
    public float BlockFallDelay; //Delay before block falls while suspended in air
    public float RaiseSpeed;
    public float BaseRaiseSpeed;
    public float ManualRaiseSpeed;
    public float RaiseAcceleration;
    TimedAction RaiseSpeedTimer;
    public float RaiseStopTime;
    public int BlockDestroyCount; //Stops Move(), but not RaiseStopTimer; Timer is created anew if time > what is left of original

    private Vector2 PopupDisplayLoc; //Set in priority of UP, LEFT, RIGHT, DOWN
    private int ComboCount; //Only Persists until next Move() (by default)
    public int ChainCount; //Once BlockDestroyCount strikes 0, reset this (by default) 
    private int PrevChainCount;
    public int FallingChainCounter;
    public int ActiveChainCounter;

    public bool IsHoldingTrigger;
    public bool IsManuallyRaising;
    public bool CanManuallyRaise;
    private float BlockDist;

    public bool AtTop;
    public float InitialBlock_Y;
    public float Target_Y; //After reaching this Y, have moved up a full "level"
    public IDictionary<Vector2, Block> BlockList;
    private IDictionary<Vector2, Block> TmpBlockList; //For transferring after moving up a "level"
    private List<Block> ComboBlockList; //All Blocks belonging to current combo
    private List<Block> ChainedBlockList; //All Blocks belonging to current chain
    public Block NullBlock;

    public BlockContainer(int maxTypes, int startingHeight, float startingSpeed)
    {
        MaxTypes = maxTypes;
        StartingHeight = startingHeight;
        BaseRaiseSpeed = RaiseSpeed = startingSpeed;
    }

    void Awake()
    {
        BlockList = new Dictionary<Vector2, Block>();
        TmpBlockList = new Dictionary<Vector2, Block>();
        ComboBlockList = new List<Block>();
        ChainedBlockList = new List<Block>();
    }

    void Start()
    {
        BlockDist = GameController.GC.BlockDist;
        InitialBlock_Y = -0.5f * BlockDist;
    }

    public void Initialize(Vector2 boardSize)
    {
        AtTop = false;
        BoardSize = boardSize;
        Target_Y = transform.localPosition.y + GameController.GC.BlockDist;
        ComboCount = ChainCount = 1;
        RaiseSpeed = BaseRaiseSpeed;
        RaiseStopTime = 0;

        if(RaiseSpeedTimer != null) {
            GameController.GC.RemoveTimedAction(RaiseSpeedTimer);
        }
        RaiseSpeedTimer = GameController.GC.AddTimedAction(() => { RaiseSpeed += RaiseAcceleration; }, 30f, true);

        SpawnRows(StartingHeight + 1, rowModVals: new List<int>(){-2, 0, 0, 2});
        ResetChain();

        CanManuallyRaise = true;
    }

    public void IncrementRaiseStopTime(float raiseStopTime)
    {
        if(raiseStopTime > RaiseStopTime) { RaiseStopTime = raiseStopTime; }
    }

    public void ResetChain()
    {
        FallingChainCounter = ActiveChainCounter = 0;
        ChainCount = 1;
        GameController.GC.UpdateGameStatMenu(ChainCount);
    }

    public void SpawnRows(int numOfRows = 1, int numOfCols = 6, int startingRow = 0, IList<int> rowModVals = null)
    {
        SpawnRows(BlockStatus.Normal, numOfRows, numOfCols, startingRow, rowModVals);
    }

    public void SpawnRows(BlockStatus status, int numOfRows = 1, int numOfCols = 6, int startingRow = 0, IList<int> rowModVals = null)
    {
        var tmpMatchList = new List<Block>();
        var searchDirs = new List<Vector2>();

        searchDirs.Add(Vector2.up); 
        searchDirs.Add(Vector2.down); 

        for(var col = 0; col < numOfCols; col++)
        {
            if(col == 2) {
                searchDirs.Add(Vector2.left); 
                searchDirs.Add(Vector2.right); 
            }

            var modRows = numOfRows;
            if(rowModVals != null)
            {
                modRows += rowModVals[Random.Range(0, rowModVals.Count-1)];
            }

            for(var row = startingRow; row < modRows; row++)
            {
                var block = BlockPooler.BP.GetPooledObject(transform).GetComponent<Block>();

                block.BoardLoc = new Vector2(col, row);
                block.Type = (BlockType)Random.Range(1, MaxTypes + 1);

                BlockList.Add(block.BoardLoc, block);

                while(MatchSurroundingBlocks(block, searchDirs, ref tmpMatchList, true)) { 
                    block.Type = (BlockType)Random.Range(1, MaxTypes + 1);
                }

                block.Initialize(row > 0 && row <= 12);
                block.gameObject.TransBySpriteDimensions(new Vector3(col - 2.5f, InitialBlock_Y + row, 0));
            }
        }
    }

    public void OnUpdate()
    {
        //Trigger Chains & Combos
        if(ChainedBlockList.Count > 0) 
        {
            ActiveChainCounter++;
            OnBlocksStartDestroy(ChainedBlockList.Distinct(), true);
            ChainedBlockList.Clear();
        }
        if(ComboBlockList.Count > 0) {
            OnBlocksStartDestroy(ComboBlockList.Distinct());
            ComboBlockList.Clear();
        }

        if(ChainCount > 1 && ActiveChainCounter == 0 && FallingChainCounter == 0) {
            ResetChain();
        }

        //Check if no destructions still, if so, reduce timestop
        if(BlockDestroyCount > 0) { return; }

        //If nothing currently being destroyed, can trigger raise (even in timestop)
        if(CanManuallyRaise && IsHoldingTrigger && !BlockList.Values.Any(x => x.IsFallLocked || x.IsFalling || x.IsMoving)) { 
            IsManuallyRaising = true;
            CanManuallyRaise = false;
            RaiseSpeed = ManualRaiseSpeed;
        }

        if(RaiseStopTime > 0) 
        {
            if(IsManuallyRaising) { RaiseStopTime = 0; } 
            else { RaiseStopTime -= Time.deltaTime; }
            
            if(RaiseStopTime > 0) { return; }
            else { RaiseStopTime = 0; }
        }

        if(AtTop) { return; }

        //Raise Blocks
        var yPos = transform.localPosition.y;

        if(yPos < Target_Y) 
        {
            if(yPos + RaiseSpeed > Target_Y)
            {
                transform.localPosition.Set(transform.localPosition.x, Target_Y, 1);
                Target_Y++;

                //Shift blocks up
                TmpBlockList = new Dictionary<Vector2, Block>();
                var comboableBlockList = new List<Block>();

                foreach(var block in BlockList)
                {
                    var nextKey = block.Key + Vector2.up;
                    TmpBlockList.Add(nextKey, block.Value);

                    if(block.Value.HasIterated) { continue; }
                    block.Value.HasIterated = true;
                    
                    block.Value.MoveBoardLoc(Vector2.up, true);
                    
                    if(!AtTop && nextKey.y >= BoardSize.y) { AtTop = Cursor.AtTop = true; }

                    if(block.Value.BoardLoc.y == 1) 
                    {
                        block.Value.OnEnterBoard();
                        comboableBlockList.Add(block.Value);
                    }
                }

                BlockList = TmpBlockList;

                foreach(var block in BlockList) {
                    block.Value.HasIterated = false;
                }

                InitialBlock_Y--;

                SpawnRows();
                OnBlocksFinishMove(comboableBlockList);

                //Shift cursor up
                Cursor.MoveBoardLoc(Vector2.up);
                if(!AtTop && Cursor.BoardLoc.y >= BoardSize.y) {
                    Cursor.OnMove(Vector2.down);
                }
                
                //If was manually moving faster, stop & wait half a second before next check
                RaiseSpeed = BaseRaiseSpeed;

                if(IsManuallyRaising) {
                    CanManuallyRaise = false;
                    RaiseSpeed = BaseRaiseSpeed;
                    GameController.GC.AddTimedAction(UnlockTrigger, 0.05f);
                }
            }
            else
            {
                transform.localPosition += new Vector3(0, RaiseSpeed, 0);
            }
        }
    }

    public void OnCursorConfirm(Vector2 cursorLoc)
    {
        Block leftBlock, rightBlock;

        //Find Blocks. If either immobile or both don't exist, return
        var leftBlockExists = BlockList.TryGetValue(cursorLoc, out leftBlock);
        if(leftBlockExists && (!leftBlock.IsMoveable || leftBlock.IsDestroying || leftBlock.IsFalling || leftBlock.IsFallLocked)) return;

        var rightBlockExists = BlockList.TryGetValue(cursorLoc + Vector2.right, out rightBlock);
        if((rightBlockExists && (!rightBlock.IsMoveable || rightBlock.IsDestroying || rightBlock.IsFalling || rightBlock.IsFallLocked)) || (!rightBlockExists && !leftBlockExists)) return;
        
        //Move Blocks if they exist & prepare for checking matches
        var checkBlockList = new List<Block>();

        if(leftBlockExists && rightBlockExists) {
            checkBlockList.Add(leftBlock);
            checkBlockList.Add(rightBlock);

            leftBlock.Move(Vector2.right);
            rightBlock.Move(Vector2.left, true, callback: () => { OnBlocksFinishMove(checkBlockList); });
            BlockList[leftBlock.BoardLoc] = leftBlock;
            BlockList[rightBlock.BoardLoc] = rightBlock;
        } 
        else if(leftBlockExists) {
            checkBlockList.Add(leftBlock);

            BlockList[leftBlock.BoardLoc] = NullBlock;

            leftBlock.Move(Vector2.right, callback: () => { 
                BlockList.Remove(leftBlock.PrevBoardLoc);
                OnBlocksFinishMove(checkBlockList); 
            });
            BlockList[leftBlock.BoardLoc] = leftBlock;
        }
        else {
            checkBlockList.Add(rightBlock);

            BlockList[rightBlock.BoardLoc] = NullBlock;

            rightBlock.Move(Vector2.left, callback: () => { 
                BlockList.Remove(rightBlock.PrevBoardLoc); 
                OnBlocksFinishMove(checkBlockList); 
            });
            BlockList[rightBlock.BoardLoc] = rightBlock;
        }
    }

    private void OnBlocksFinishMove(List<Block> checkBlockList, bool isFromFall = false)
    {
        var matchingList = new List<Block>();
        var chainRemovalList = new List<Block>();
        var isPassingChain = false;

        if(isFromFall) {
            checkBlockList.ForEach(x => x.HasIterated = true);
        }

        foreach(var checkBlock in checkBlockList)
        {
            var checkForMatches = true;

            checkBlock.IsFalling = false;
            checkBlock.SetStates(true);

            //Fall if no block under
            var blockBelowPos = new Vector2(checkBlock.BoardLoc.x, checkBlock.BoardLoc.y - 1);
            if(!BlockList.ContainsKey(blockBelowPos)) 
            {
                LockBlocksAboveLoc(blockBelowPos, checkBlock.IsChainable);
                checkForMatches = false;
            }

            //Drop any that were above
            if(checkBlockList.Count == 1) 
            {
                var prevLoc = new Vector2(checkBlock.PrevBoardLoc.x, checkBlock.PrevBoardLoc.y);
                LockBlocksAboveLoc(prevLoc);
            }

            //If from fall, have to be on "grounded" block to match; Else pass chain if needed
            if(isFromFall && checkBlock.BoardLoc.y > 1)
            {
                var onGround = IsGroundedBlockBelowLoc(checkBlock.BoardLoc);
                if(!onGround) 
                { 
                    checkForMatches = false; 
                    isPassingChain = true;
                    Debug.Log("Passing Chain");
                }
            }

            if(checkForMatches)
            {
                //Don't check in the direction the block moved from
                var reverseDirVector = checkBlock.PrevBoardLoc - checkBlock.BoardLoc;
                var searchDirs = new List<Vector2>() { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
                searchDirs.Remove(new Vector2(reverseDirVector.x, reverseDirVector.y));

                MatchSurroundingBlocks(checkBlock, searchDirs, ref matchingList);
            }

            if(checkBlock.IsChainable && !isPassingChain) 
            {
                chainRemovalList.Add(checkBlock);
            }
        }

        if(isFromFall) {
            checkBlockList.ForEach(x => x.HasIterated = false);
        }

        //Add matches to chain or combo list ELSE end chain if appropriate
        if(matchingList.Count >= 3) 
        {
            matchingList.ForEach(x => x.IsMoveable = false);
            
            if(matchingList.Exists(x => x.IsChainable)) 
            {
                ChainedBlockList.AddRange(matchingList);
            } 
            else 
            {
                ComboBlockList.AddRange(matchingList);
            }
        }

        //After a short amount of time, remove the "chainability" of blocks if needed
        if(chainRemovalList.Count > 0) 
        {
            GameController.GC.AddTimedAction(() => { 
                foreach(var chainRemovalBlock in chainRemovalList) {
                    chainRemovalBlock.IsChainable = false;
                }
                if(FallingChainCounter > 0 && !isPassingChain) { 
                    FallingChainCounter--; 
                }
            }, GameController.GC.BlockSwitchSpeed / 2);
        }
    }

    private void OnBlocksStartDestroy(IEnumerable<Block> blocksToDestroy, bool isChain = false)
    {
        //Order: Top to Bottom -> Left to Right
        var destroyBlockList = blocksToDestroy.OrderByDescending(a => a.BoardLoc.y).ThenBy(a => a.BoardLoc.x).ToList();

        //Increment variables relating to destroying blocks AND add to/start a chain
        var comboCount = destroyBlockList.Count();
        BlockDestroyCount += comboCount;

        var totalRaiseTimeStop = 0.5f + ((comboCount - 3) * GameController.GC.RaiseTimeStopComboMultiplier);

        if(isChain)  
        {
            ChainCount += 1;
            GameController.GC.UpdateGameStatMenu(ChainCount);
            totalRaiseTimeStop += ChainCount * GameController.GC.RaiseTimeStopChainMultiplier;
        }

        IncrementRaiseStopTime(totalRaiseTimeStop);
    
        //Lead block store next destroy phase
        var lastBlock = destroyBlockList.Last();
        lastBlock.StoredAction = () => { OnBlocksIconDestroy(destroyBlockList, isChain); };

        //TODO: Use the 1st block in this list as the point at which to display the count (pass through or do here)
        foreach(var block in destroyBlockList) {
            block.StartDestroy();
        }
    }

    private void OnBlocksIconDestroy(List<Block> destroyBlockList, bool isChain = false)
    {
        for(var i = 0; i < destroyBlockList.Count; i++)
        {
            var block = destroyBlockList[i];
            block.StoredAction = null;

            if(i == 0) {
                block.IconDestroy();
            } else {
                GameController.GC.AddTimedAction(block.IconDestroy, IconDestroyDelay * i);
            }

            if(i == destroyBlockList.Count - 1) {
                block.StoredAction = () => { 
                    GameController.GC.AddTimedAction(() => { OnBlocksFinishDestroy(destroyBlockList, isChain); }, 0.2f);
                };
            }
        }
    }

    private void OnBlocksFinishDestroy(List<Block> destroyBlockList, bool isChain = false)
    {
        var fallLocList = new List<Vector2>();

        foreach(var xGroupBlockList in destroyBlockList.GroupBy(x => x.BoardLoc.x))
        {
            var prevY = 0f;

            foreach(var blockToDestroy in xGroupBlockList)
            {
                //Add to fall list if unique column or is greater than 1 away from existing rows
                if(!fallLocList.Exists(loc => loc.x == blockToDestroy.BoardLoc.x && Mathf.Abs(prevY - blockToDestroy.BoardLoc.y) <= 1))
                {
                    fallLocList.Add(blockToDestroy.BoardLoc);
                }

                //Actually remove the Block
                BlockList.Remove(blockToDestroy.BoardLoc);

                //TODO: Remove this safety check if not used
                foreach(var leftoverBlock in BlockList.Where(x => x.Value.GetInstanceID() == blockToDestroy.GetInstanceID()).ToList())
                {
                    BlockList.Remove(leftoverBlock.Key);
                    Debug.LogWarning("Leftover Block removed at: " + leftoverBlock.Key);
                }

                prevY = blockToDestroy.BoardLoc.y;
                blockToDestroy.gameObject.SetActive(false);
            }
        }

        //Decrement BlockDestroy Count
        BlockDestroyCount -= destroyBlockList.Count;

        //Drop blocks above the destroyed blocks
        LockBlocksAboveLoc(fallLocList, true, isChain);
    }

    private void LockBlocksAboveLoc(IList<Vector2> boardLocList, bool isChainable = false, bool isFromChain = false)
    {
        var dropCount = 0;

        for(var i = 0; i < boardLocList.Count; i++)
        {
            if(LockBlocksAboveLoc(boardLocList[i], isChainable)) { dropCount++; }
        }

        if(isFromChain)
        {
            FallingChainCounter += dropCount;
            ActiveChainCounter--;
        }
    }

    private bool LockBlocksAboveLoc(Vector2 boardLoc, bool isChainable = false)
    {
        var blockToLockList = GetBlocksAboveLoc(boardLoc, canBeFallLocked: true);
        var nullBlockLocList = new List<Vector2>();

        if(blockToLockList.Count == 0) { return false; }

        var wasFallLocked = blockToLockList.Exists(x => x.IsFallLocked);
        //Block prevLockedBlock = null;

        foreach(var blockToLock in blockToLockList) 
        {
            if(!blockToLock.IsDestroying && !blockToLock.IsFalling) 
            {
                var nextLoc = blockToLock.BoardLoc + Vector3.down;

                Block blockBelow;
                var isBlockBelow = BlockList.TryGetValue(nextLoc, out blockBelow);

                //if(blockToLock.IsMoving && !isBlockBelow) { continue; }

                if(isBlockBelow && blockBelow.IsDestroying) { continue; }

                blockToLock.FallLockCount++;

                if(!isBlockBelow) 
                { 
                    BlockList[nextLoc] = NullBlock;
                    nullBlockLocList.Add(nextLoc);
                }

                // if(!BlockList.ContainsKey(nextLoc) || BlockList[nextLoc] == prevLockedBlock) {
                //     blockToLock.IsFallLocked = true;
                //     BlockList[nextLoc] = blockToLock;
                //     prevLockedBlock = blockToLock;
                // }
            }
        }

        GameController.GC.AddTimedAction(() => { 
            foreach(var nullBlockLoc in nullBlockLocList) {
                if(BlockList[nullBlockLoc] == NullBlock) { BlockList.Remove(nullBlockLoc); }
            }
            foreach(var blockToUnlock in blockToLockList) {
                blockToUnlock.RemoveFallLock();
            }
            DropBlocksAboveLoc(boardLoc, isChainable);
        }, BlockFallDelay);

        return true;
    }

    private void DropBlocksAboveLoc(Vector2 boardLoc, bool isChainable = false)
    {
        var blockToFallList = GetBlocksAboveLoc(boardLoc);
        OnBlockStartFall(blockToFallList, isChainable);
    }

    //Returns consecutive blocks above a location (spaces are separated in individual lists)
    private List<Block> GetBlocksAboveLoc(Vector2 boardLoc, bool onlyFirstRow = false, bool canBeFallLocked = false)
    {
        var maxRow = onlyFirstRow ? boardLoc.y + 1 : BoardSize.y;
        var currentList = new List<Block>();
    
        for(var row = boardLoc.y; row < maxRow; row++)
        {
            boardLoc += Vector2.up;

            if(BlockList.ContainsKey(boardLoc))
            {
                var block = BlockList[boardLoc];

                //TODO: Replace logic for garbage? Still need to get blocks to fall if the whole garbage is falling too
                if(!block.CanFall()) {
                    break;
                }

                if(!currentList.Contains(block) && !block.IsFalling && (!block.IsFallLocked || canBeFallLocked)) 
                {
                    currentList.Add(BlockList[boardLoc]);
                }
            }
        }

        return currentList;
    }

    private bool IsGroundedBlockBelowLoc(Vector2 boardLoc)
    {
        Block groundBlock;

        for(var row = boardLoc.y-1; row > 0; row--)
        {
            BlockList.TryGetValue(new Vector2(boardLoc.x, row), out groundBlock);

            if(groundBlock == null || groundBlock.IsFallLocked || groundBlock.IsFalling) 
            {
                return false;
            } 
            else if(!groundBlock.IsMoving && !groundBlock.HasIterated) 
            {
                return true;
            }
        }

        return true;
    }

    private void OnBlockStartFall(List<Block> fallingBlockList, bool isChainable = false)
    {
        if(fallingBlockList.Count == 0) { return; }

        var leadFallBlock = fallingBlockList.First();

        for(;;)
        {
            //Cancel the fall if there is now a block underneath that is not the Placeholder (NullBlock)
            var leadOpenLoc = new Vector2(leadFallBlock.BoardLoc.x, leadFallBlock.BoardLoc.y - 1);
            if(leadFallBlock.IsDestroying || leadFallBlock.IsFalling || 
                (BlockList.ContainsKey(leadOpenLoc) && BlockList[leadOpenLoc] != NullBlock))
            {
                fallingBlockList.Remove(leadFallBlock);
                leadFallBlock = fallingBlockList.FirstOrDefault();

                if(leadFallBlock == null) { return; }
            }
            else 
            {
                break;
            }
        }

        var linkedBlocks = fallingBlockList.Count > 1 ? fallingBlockList.Skip(1).ToList() : null;
        var removeFlag = false;

        foreach(var fallBlock in fallingBlockList) 
        {
            if(fallBlock.IsDestroying || fallBlock.IsFalling) { removeFlag = true; }
            if(removeFlag && linkedBlocks != null && linkedBlocks.Contains(fallBlock)) { 
                linkedBlocks.Remove(fallBlock); 
            }
        }

        leadFallBlock.StartFall(isChainable, linkedBlocks, () => { OnBlocksFinishMove(fallingBlockList, true); });
    }

    private bool MatchSurroundingBlocks(Block block, IEnumerable<Vector2> searchDirs, ref List<Block> matchingList, bool ignoreState = false)
    {
        var matchingLoc = new Vector2(block.BoardLoc.x, block.BoardLoc.y);

        var xMatchList = new List<Block>();
        var yMatchList = new List<Block>();

        foreach(var searchDir in searchDirs)
        {
            var searchLoc = searchDir;
            var depth = 2;
            var matchingBlock = block;

            while(depth > 0) {
                Block blockToCheck;

                if(BlockList.TryGetValue(matchingLoc + searchLoc, out blockToCheck) && blockToCheck.IsMatch(matchingBlock, ignoreState))
                {
                    matchingBlock = blockToCheck;

                    if(searchLoc.y == 0) {
                        xMatchList.Add(blockToCheck);
                        searchLoc = searchLoc + searchLoc;
                    } else {
                        yMatchList.Add(blockToCheck);
                        searchLoc = searchLoc + searchLoc;
                    }
                }

                depth--;
            }
        }

        var comboFlag = false;

        if(xMatchList.Count >= 2) {
            if(!comboFlag) { comboFlag = true; }
            matchingList.AddRange(xMatchList);
        }
        if(yMatchList.Count >= 2) {
            if(!comboFlag) { comboFlag = true; }
            matchingList.AddRange(yMatchList);
        }
        if(comboFlag) { matchingList.Add(block); }

        return comboFlag;
    }

    //Start or Stop manual speed increase
    public void OnTrigger(bool performed)
    {
        if(performed && !IsHoldingTrigger) {
            IsHoldingTrigger = true;
        } else if(!performed && IsHoldingTrigger) {
            IsHoldingTrigger = false;
        }
    }

    private void UnlockTrigger()
    {
        CanManuallyRaise = true;
    }
}