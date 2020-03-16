using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    public GameCursor Cursor;
    public Vector2 BoardSize;
    private GameBoard PlayerGameBoard;
    public int MaxTypes;
    public int StartingHeight;
    public float IconDestroyDelay; //Delay between icons being destroyed; Reduce for faster Gameplay
    public float ChainLinkDelay; //Delay between starting/adding to a chain VS starting a new chain (should allow for 2 diff rows being diff chains)
    public float BlockFallDelay; //Delay before block falls while suspended in air

    public float RaiseSpeed;
    public float BaseRaiseSpeed;
    public float ManualRaiseSpeed; //TODO: Move to GC
    public float RaiseAcceleration;
    private int SpeedLv;
    TimedAction RaiseSpeedLvTimer;
    //public float RaiseStopTime;
    public int BlockDestroyCount; //Stops Move(), but not RaiseStopTimer; Timer is created anew if time > what is left of original

    private Vector2 PopupDisplayLoc; //Set in priority of UP, LEFT, RIGHT, DOWN
    //private int ComboCount; //Only Persists until next Move() (by default)
    public int ChainCount; //Once BlockDestroyCount strikes 0, reset this (by default) 
    private int PrevChainCount;
    public int ActiveChainCounter;

    public bool IsHoldingTrigger;
    public bool IsManuallyRaising;
    public bool CanManuallyRaise;
    public bool AtTop;
    public float InitialBlock_Y;
    public float Target_Y; //After reaching this Y, have moved up a full "level"
    public IDictionary<Vector2, Block> BlockList;
    private IDictionary<Vector2, Block> TmpBlockList; //For transferring after moving up a "level"
    private List<Block> PotentialChainBlockList; //Keep track of potential chain blocks (once empty, can reset chain)
    private List<Block> ComboBlockList; //All Blocks belonging to current combo
    private List<Block> ChainedBlockList; //All Blocks belonging to current chain

    public Block NullBlock;
    private float BlockDist;

    public BlockContainer(int maxTypes, int startingHeight, float startingSpeed)
    {
        MaxTypes = maxTypes;
        StartingHeight = startingHeight;
        BaseRaiseSpeed = RaiseSpeed = startingSpeed;
    }

    public void Initialize(Vector2 boardSize, float baseRaiseSpeed)
    {
        BlockList = new Dictionary<Vector2, Block>();
        TmpBlockList = new Dictionary<Vector2, Block>();
        PotentialChainBlockList = new List<Block>();
        ComboBlockList = new List<Block>();
        ChainedBlockList = new List<Block>();

        PlayerGameBoard = GameController.GameCtrl.PlayerGameBoard;

        BlockDist = GameController.GameCtrl.BlockDist;
        InitialBlock_Y = -0.5f * BlockDist;

        AtTop = false;
        BoardSize = boardSize;
        Target_Y = transform.localPosition.y + GameScoreAtkCtrl.GameSA_Ctrl.BlockDist;
        RaiseSpeed = BaseRaiseSpeed = baseRaiseSpeed;
        //RaiseStopTime = 0;
        //ComboCount = 0;
        ChainCount = 1;

        SpawnRows(StartingHeight + 1, rowModVals: new List<int>(){-1, 0, 0, 1});
        ResetChain();

        CanManuallyRaise = true;
    }

    public void IncreaseSpeed(float nextSpeed)
    {
        BaseRaiseSpeed = nextSpeed;
        if(!IsManuallyRaising) { RaiseSpeed = BaseRaiseSpeed; }
    }

    public void ResetChain()
    {
        ActiveChainCounter = 0;
        ChainCount = 1;
        //GameController.GameCtrl.UpdateGameStatMenu(ChainCount);
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
        if(ComboBlockList.Count > 0) 
        {
            OnBlocksStartDestroy(ComboBlockList.Distinct());
            ComboBlockList.Clear();
        }
        if(PotentialChainBlockList.Count > 0)
        {
            PotentialChainBlockList.RemoveAll(x => !x.IsFallLocked && !x.IsFalling && !x.IsChainable);
        }
        if(ChainCount > 1 && ActiveChainCounter <= 0 && PotentialChainBlockList.Count == 0) 
        {
            ResetChain();
        }

        //Check Top Row
        bool isTopRow = false;

        for(var x = 0; x < BoardSize.x; x++)
        {
            if(BlockList.ContainsKey(new Vector2(x, BoardSize.y))) { 
                isTopRow = true; 
                break;
            }
        }

        //Adjust 'AtTop' and TimeBar Visibility
        if(isTopRow && !AtTop) 
        {
            AtTop = true;
            PlayerGameBoard.TimeStopper.ShowBar();
        } 
        else if(!isTopRow && AtTop) 
        {
            AtTop = false;
            PlayerGameBoard.TimeStopper.HideBar();
        }

        //Check if no destructions still, if so, reduce timestop
        if(BlockDestroyCount > 0 || PotentialChainBlockList.Count > 0) { return; }

        //Trigger Check
        if(!AtTop && CanManuallyRaise && IsHoldingTrigger && !BlockList.Values.Any(x => x.IsFallLocked || x.IsFalling || x.IsMoving)) { 
            IsManuallyRaising = true;
            CanManuallyRaise = false;
            RaiseSpeed = ManualRaiseSpeed;

            PlayerGameBoard.TimeStopper.ResetTime();
        }

        //Timestop Check/Decrement
        if(PlayerGameBoard.TimeStopper.IsTimeStopped && PlayerGameBoard.TimeStopper.UpdateTime(Time.deltaTime)) { return; }

        if(isTopRow && AtTop) {
            GameController.GameCtrl.LoseGame(); 
            return;
        }

        //if(AtTop || transform.localPosition.y >= Target_Y) { return; }

        OnMoveBoard();
    }

    private void OnMoveBoard()
    {
        //Raise Blocks
        var yPos = transform.localPosition.y;

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
                
                //Have reached the top with normal blocks
                if(!AtTop && nextKey.y >= BoardSize.y) 
                { 
                    if(!PlayerGameBoard.TimeStopper.IsTimeStopped) { PlayerGameBoard.TimeStopper.AddTime(3, 1); }
                }

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
                Cursor.OnMove(Vector2.down, false);
            }
            
            //If was manually moving faster, stop & wait half a second before next check
            RaiseSpeed = BaseRaiseSpeed;

            if(IsManuallyRaising) {
                CanManuallyRaise = false;
                IsManuallyRaising = false;
                MainController.MC.AddTimedAction(UnlockManualRaise, 0.05f);
            }
        }
        else
        {
            transform.localPosition += new Vector3(0, RaiseSpeed, 0);
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

        MainController.MC.PlaySound("BlockMove");
    }

    private void OnBlocksFinishMove(List<Block> checkBlockList, bool isFromFall = false)
    {
        var matchingList = new List<Block>();
        var chainRemovalList = new List<Block>();
        var isPassingChain = false;

        foreach(var checkBlock in checkBlockList)
        {
            var checkForMatches = true;

            checkBlock.IsFalling = false;
            checkBlock.IsMoveable = checkBlock.IsComboable = true;

            //Fall if no block under
            var blockBelowPos = new Vector2(checkBlock.BoardLoc.x, checkBlock.BoardLoc.y - 1);
            if(!BlockList.ContainsKey(blockBelowPos) && !checkBlock.IsFallLocked) 
            {
                checkBlock.IsComboable = false;
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
                }
            }

            if(checkForMatches)
            {
                //Don't check in the direction the block moved from
                var reverseDirVector = checkBlock.PrevBoardLoc - checkBlock.BoardLoc;
                var searchDirs = new List<Vector2>() { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
                searchDirs.Remove(new Vector2(reverseDirVector.x, reverseDirVector.y));

                MatchSurroundingBlocks(checkBlock, searchDirs, ref matchingList, searchDepth: isFromFall ? 5 : 2);
            }

            if(checkBlock.IsChainable && !isPassingChain) 
            {
                chainRemovalList.Add(checkBlock);
            }
        }

        //Add matches to chain or combo list ELSE end chain if appropriate
        if(matchingList.Count >= 3) 
        {
            //Need to add "fresh" to the list (was added on top of existing chain/combo)
            if(matchingList.Exists(x => x.IsComboing == true))
            {
                ChainedBlockList.RemoveAll(x => matchingList.Contains(x));
                ComboBlockList.RemoveAll(x => matchingList.Contains(x));
            }

            matchingList.ForEach(x => x.StartComboing());
            
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
            MainController.MC.AddTimedAction(() => { 
                foreach(var chainRemovalBlock in chainRemovalList) {
                    if(chainRemovalBlock.IsChainable)
                    {
                        chainRemovalBlock.IsChainable = false;
                    }
                }
            }, GameController.GameCtrl.BlockSwitchSpeed * 0.8f);
        }
    }

    private void OnBlocksStartDestroy(IEnumerable<Block> blocksToDestroy, bool isChain = false)
    {
        //Order: Top to Bottom -> Left to Right
        var destroyBlockList = blocksToDestroy.OrderByDescending(a => a.BoardLoc.y).ThenBy(a => a.BoardLoc.x).ToList();

        //Increment variables relating to destroying blocks AND add to/start a chain
        var comboCount = destroyBlockList.Count();
        BlockDestroyCount += comboCount;

        var totalRaiseTimeStop = GameController.GameCtrl.RaiseTimeStopBaseComboAmount + ((comboCount - 3) * GameController.GameCtrl.RaiseTimeStopComboMultiplier);

        if(isChain)  
        {
            ChainCount += 1;
            //GameController.GameCtrl.UpdateGameStatMenu(ChainCount);
            totalRaiseTimeStop += ChainCount * GameController.GameCtrl.RaiseTimeStopChainMultiplier;
        }

        PlayerGameBoard.IncreaseScore(comboCount, isChain ? ChainCount : 1);
        PlayerGameBoard.TimeStopper.AddTime(comboCount, isChain ? ChainCount : 0);
    
        var firstBlockFlag = destroyBlockList.Count > 3 || isChain;

        //TODO: Use the 1st block in this list as the point at which to display the count (pass through or do here)
        for(var i = 0; i < destroyBlockList.Count; i++)
        {
            var block = destroyBlockList[i];

            if(i == 0 && firstBlockFlag) { 
                block.StartDestroy(destroyBlockList.Count, isChain ? ChainCount : 1); 
            }
            else if(i < destroyBlockList.Count - 1) { 
                block.StartDestroy(); 
            }
            else {
                block.StartDestroy(() => { OnBlocksIconDestroy(destroyBlockList, isChain); });
            }
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
                MainController.MC.AddTimedAction(block.IconDestroy, IconDestroyDelay * i);
            }

            if(i == destroyBlockList.Count - 1) {
                block.StoredAction = () => { 
                    MainController.MC.AddTimedAction(() => { OnBlocksFinishDestroy(destroyBlockList, isChain); }, 0.2f);
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
                BlockPooler.BP.RepoolBlock(blockToDestroy);
            }
        }

        //Decrement BlockDestroy Count
        BlockDestroyCount -= destroyBlockList.Count;

        //Drop blocks above the destroyed blocks
        LockBlocksAboveLoc(fallLocList, true, isChain);
    }

    //Individual Falling Blocks version
    private void LockBlocksAboveLoc(IList<Vector2> boardLocList, bool isChainable = false, bool isFromChain = false)
    {
        List<Block> blockLockList = new List<Block>();

        for(var i = 0; i < boardLocList.Count; i++)
        {
            blockLockList.AddRange(LockBlocksAboveLoc(boardLocList[i], isChainable));
        }

        if(isFromChain)
        {
            ActiveChainCounter--;
            PotentialChainBlockList.AddRange(blockLockList);
        }
    }

    //Individual Falling Blocks version
    private List<Block> LockBlocksAboveLoc(Vector2 boardLoc, bool isChainable = false)
    {
        var blockToLockList = GetBlocksAboveLoc(boardLoc, canBeFallLocked: true);
        var nullBlockLocList = new List<Vector2>();

        if(blockToLockList.Count == 0) { return blockToLockList; }

        foreach(var blockToLock in blockToLockList) 
        {
            if(!blockToLock.IsDestroying && !blockToLock.IsFalling && !blockToLock.IsMoving) 
            {
                var nextLoc = blockToLock.BoardLoc + Vector3.down;

                Block blockBelow;
                var isBlockBelow = BlockList.TryGetValue(nextLoc, out blockBelow);

                if(isBlockBelow && blockBelow.IsDestroying) { continue; }

                blockToLock.IsFallLocked = true;

                if(!isBlockBelow) 
                { 
                    BlockList[nextLoc] = NullBlock;
                    nullBlockLocList.Add(nextLoc);
                }
            }
        }

        var prevTargetY = Target_Y;

        MainController.MC.AddTimedAction(() => { 
            foreach(var nullBlockLoc in nullBlockLocList) {
                if(BlockList.ContainsKey(nullBlockLoc) && BlockList[nullBlockLoc].GetInstanceID() == NullBlock.GetInstanceID()) { BlockList.Remove(nullBlockLoc); }
            }
            foreach(var blockToUnlock in blockToLockList) {
                blockToUnlock.RemoveFallLock();
            }
            DropBlocksAboveLoc(boardLoc, prevTargetY, isChainable);
        }, BlockFallDelay);

        return blockToLockList;
    }

    private void DropBlocksAboveLoc(Vector2 boardLoc, float prevTargetY, bool isChainable = false)
    {
        while(prevTargetY < Target_Y) {
            boardLoc += Vector2.up;
            prevTargetY++;
        }

        var blockToFallList = GetBlocksAboveLoc(boardLoc);
        if(isChainable) { PotentialChainBlockList.AddRange(blockToFallList); }

        OnBlockStartFall(blockToFallList, isChainable);
    }

    //Returns consecutive blocks above a location (stops if runs into "grounded")
    private List<Block> GetBlocksAboveLoc(Vector2 boardLoc, bool onlyFirstRow = false, bool canBeFallLocked = false)
    {
        var maxRow = onlyFirstRow ? boardLoc.y + 1 : BoardSize.y;
        var blockAboveList = new List<Block>();
    
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

                if(!blockAboveList.Exists(x => x.GetInstanceID() == block.GetInstanceID()) && !block.IsFalling && (!block.IsFallLocked || canBeFallLocked)) 
                {
                    blockAboveList.Add(BlockList[boardLoc]);
                }
            }
        }

        return blockAboveList;
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
        }

        return true;
    }

    //Individual Falling Blocks (Must be in bottom to top order to work)
    private void OnBlockStartFall(List<Block> fallingBlockList, bool isChainable = false)
    {
        if(fallingBlockList.Count == 0) { return; }

        foreach(var fallBlock in fallingBlockList) 
        {
            var firstFallLoc = new Vector2(fallBlock.BoardLoc.x, fallBlock.BoardLoc.y - 1);

            if(!(fallBlock.IsDestroying || fallBlock.IsFalling || 
                (BlockList.ContainsKey(firstFallLoc) && !BlockList[firstFallLoc].IsFalling))) 
            {  
                fallBlock.StartFall(fallBlock.IsChainable ? true : isChainable, callback: () => { OnBlocksFinishMove(fallingBlockList, true); });
            }
        }
    }

    private bool MatchSurroundingBlocks(Block block, IEnumerable<Vector2> searchDirs, ref List<Block> matchingList, bool ignoreState = false, int searchDepth = 2)
    {
        var matchingLoc = new Vector2(block.BoardLoc.x, block.BoardLoc.y);

        var xMatchList = new List<Block>();
        var yMatchList = new List<Block>();

        foreach(var searchDir in searchDirs)
        {
            var searchLoc = searchDir;
            var depth = searchDepth;
            var matchingBlock = block;

            while(depth > 0) {
                Block blockToCheck;

                if(BlockList.TryGetValue(matchingLoc + searchLoc, out blockToCheck) && blockToCheck.IsMatch(matchingBlock, ignoreState))
                {
                    matchingBlock = blockToCheck;

                    if(searchLoc.y == 0) {
                        xMatchList.Add(blockToCheck);
                        searchLoc = searchLoc + searchLoc.normalized;
                    } else {
                        yMatchList.Add(blockToCheck);
                        searchLoc = searchLoc + searchLoc.normalized;
                    }
                }
                else {
                    depth = 0;
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
    public void OnInputTrigger(bool performed)
    {
        if(performed && !IsHoldingTrigger) {
            IsHoldingTrigger = true;
        } else if(!performed && IsHoldingTrigger) {
            IsHoldingTrigger = false;

            if(IsManuallyRaising && PlayerGameBoard.TimeStopper.IsTimeStopped) {
                IsManuallyRaising = false;
                CanManuallyRaise = true;
                RaiseSpeed = BaseRaiseSpeed;
            }
        }
    }

    private void UnlockManualRaise()
    {
        CanManuallyRaise = true;
    }
}