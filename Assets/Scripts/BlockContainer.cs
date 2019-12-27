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
    public float BlockSwitchSpeed; //Speed at which blocks can be switched
    public float Speed;
    public float BaseSpeed;
    public float ManualRaiseSpeed;
    TimedAction RaiseStopTimer;
    public float RaiseStopComboMultiplier; //RaiseStopTimer Time += (ComboCount * Multiplier)
    public float RaiseStopChainMultiplier; //RaiseStopTimer Time += (ChainCount * Multiplier)
    public int BlockDestroyCount; //Stops Move(), but not RaiseStopTimer; Timer is created anew if time > what is left of original

    private Vector2 PopupDisplayLoc; //Set in priority of UP, LEFT, RIGHT, DOWN
    public int ComboCount; //Only Persists until next Move() (by default)
    public int ChainCount; //Once BlockDestroyCount strikes 0, reset this (by default) 
    private bool IsHoldingTrigger;
    private bool IsManuallyRaising;
    private float BlockDist;

    public bool AtTop;
    public float InitialBlock_Y;
    public float Target_Y; //After reaching this Y, have moved up a full "level"
    public IDictionary<Vector2, Block> BlockList;
    private IDictionary<Vector2, Block> TmpBlockList; //For transferring after moving up a "level"
    private List<Block> ComboBlockList; //All Blocks belonging to current combo
    private IList<Block> ChainedBlockList; //All Blocks belonging to current chain
    //private bool IsComboActive;

    public BlockContainer(int maxTypes, int startingHeight, float startingSpeed)
    {
        MaxTypes = maxTypes;
        StartingHeight = startingHeight;
        BaseSpeed = Speed = startingSpeed;
    }

    void Awake()
    {
        AtTop = false;
        BlockList = new Dictionary<Vector2, Block>();
        TmpBlockList = new Dictionary<Vector2, Block>();
        ComboBlockList = new List<Block>();
    }

    void Start()
    {
        BlockDist = GameController.GC.BlockDist;
        InitialBlock_Y = -0.5f * BlockDist;
    }

    public void Initialize(Vector2 boardSize)
    {
        BoardSize = boardSize;
        Target_Y = transform.localPosition.y + GameController.GC.BlockDist;
        ComboCount = ChainCount = 0;

        SpawnRows(StartingHeight + 1, rowModVals: new List<int>(){-2, 0, 0, 2}, isComboable: false);
    }

    public void SpawnRows(int numOfRows = 1, int numOfCols = 6, int startingRow = 0, IList<int> rowModVals = null, bool isComboable = true)
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

                while(CheckSurroundingBlocks(block, searchDirs, ref tmpMatchList, true)) { 
                    block.Type = (BlockType)Random.Range(1, MaxTypes + 1);
                }

                block.Initialize(row > 0 && row <= 12);
                block.gameObject.TransBySpriteDimensions(new Vector3(col - 2.5f, InitialBlock_Y + row, 0));
            }
        }
    }

    public void Move()
    {
        if(ComboBlockList.Count > 0) {
            OnBlocksStartDestroy(ComboBlockList.Distinct());
            ComboBlockList.Clear();
        }
        if(BlockDestroyCount > 0) { return; }
        if(!AtTop)
        {
            var yPos = transform.localPosition.y;

            if(yPos < Target_Y) 
            {
                if(yPos + Speed > Target_Y)
                {
                    transform.localPosition.Set(transform.localPosition.x, Target_Y, 1);
                    Target_Y++;

                    //Shift container down
                    //transform.localPosition = new Vector3(0, Target_Y - BlockDist, 0);

                    //Shift blocks up
                    TmpBlockList = new Dictionary<Vector2, Block>();
                    var comboableBlockList = new List<Block>();

                    foreach(var block in BlockList)
                    {
                        if(block.Value.HasIterated) { continue; }
                        block.Value.HasIterated = true;

                        var nextKey = block.Key + Vector2.up;
                        
                        if(!AtTop && nextKey.y >= BoardSize.y)
                        {
                            AtTop = Cursor.AtTop = true;
                        }

                        TmpBlockList.Add(nextKey, block.Value);
                        block.Value.MoveBoardLoc(Vector2.up, true);

                        if(block.Value.BoardLoc.y == 1) {
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
                    if(IsManuallyRaising) {
                        Speed = BaseSpeed;
                        GameController.GC.AddTimedAction(UnlockTrigger, 0.05f);
                    }
                }
                else
                {
                    transform.localPosition += new Vector3(0, Speed, 0);
                }
            }
        }
    }

    public void OnCursorConfirm(Vector2 cursorLoc)
    {
        Block leftBlock, rightBlock;

        //Find Blocks. If either immobile or both don't exist, return
        var leftBlockExists = BlockList.TryGetValue(cursorLoc, out leftBlock);
        if(leftBlockExists && !leftBlock.IsMoveable && !leftBlock.IsFalling) return;

        var rightBlockExists = BlockList.TryGetValue(cursorLoc + Vector2.right, out rightBlock);
        if((rightBlockExists && !rightBlock.IsMoveable && !rightBlock.IsFalling) || (!rightBlockExists && !leftBlockExists)) return;
        
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

            leftBlock.Move(Vector2.right, callback: () => { 
                BlockList.Remove(leftBlock.PrevBoardLoc); 
                OnBlocksFinishMove(checkBlockList); 
            });
            BlockList[leftBlock.BoardLoc] = leftBlock;
        }
        else {
            checkBlockList.Add(rightBlock);

            rightBlock.Move(Vector2.left, callback: () => { 
                BlockList.Remove(rightBlock.PrevBoardLoc); 
                OnBlocksFinishMove(checkBlockList); 
            });
            BlockList[rightBlock.BoardLoc] = rightBlock;
        }
    }

    private void OnBlocksFinishMove(List<Block> checkBlockList, bool fromFall = false)
    {
        var matchingList = new List<Block>();

        foreach(var checkBlock in checkBlockList)
        {
            checkBlock.IsFalling = false;
            checkBlock.SetStates(true);

            //If no Block beneath, start falling
            var blockBelowPos = new Vector2(checkBlock.BoardLoc.x, checkBlock.BoardLoc.y - 1);
            if(!BlockList.ContainsKey(blockBelowPos)) 
            {
                checkBlock.IsMoveable = checkBlock.IsComboable = false;
                GameController.GC.AddTimedAction(() => { DropBlocksAboveLoc(blockBelowPos); }, BlockFallDelay);
            }

            //If was the only block moved, may have other blocks that need to drop
            //NOTE: Will "Lock" Block to fall into that spot (cannot switch another in within delay)
            if(checkBlockList.Count == 1) 
            {
                var prevLoc = new Vector2(checkBlock.PrevBoardLoc.x, checkBlock.PrevBoardLoc.y);
                var lockedBlock = LockBlockAboveLoc(prevLoc);

                GameController.GC.AddTimedAction(() => { 
                    if(lockedBlock != null) {
                        lockedBlock.RemoveFallLock();
                    }
                    DropBlocksAboveLoc(prevLoc);
                } , BlockFallDelay);
            }

            //If from fall, make sure you're on a "grounded" block
            if(checkBlock.IsComboable && (!fromFall || checkBlock.BoardLoc.y <= 1 || 
                BlockList.ContainsKey(new Vector2(checkBlock.BoardLoc.x, checkBlock.BoardLoc.y - 2))))
            {
                //Don't check in the direction the block moved from
                var reverseDirVector = checkBlock.PrevBoardLoc - checkBlock.BoardLoc;
                var searchDirs = new List<Vector2>() { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
                searchDirs.Remove(new Vector2(reverseDirVector.x, reverseDirVector.y));

                CheckSurroundingBlocks(checkBlock, searchDirs, ref matchingList);
            }

            //TODO: Delay this removal
            checkBlock.IsChainable = false;
        }
        if(matchingList.Count >= 3) 
        {
            ComboBlockList.AddRange(matchingList);
        }
    }

    //Start Flashing White Blocks
    private void OnBlocksStartDestroy(IEnumerable<Block> blocksToDestroy)
    {
        //Order: Top to Bottom -> Left to Right
        var destroyBlockList = blocksToDestroy.OrderByDescending(a => a.BoardLoc.y).ThenBy(a => a.BoardLoc.x).ToList();

        //Increment Block Destroy Count (for when to start counting down time)
        BlockDestroyCount += destroyBlockList.Count();
    
        //Lead block store next destroy phase
        destroyBlockList.Last().StoredAction = () => { OnBlocksIconDestroy(destroyBlockList); };

        foreach(var block in destroyBlockList) {
            block.StartDestroy();
        }
    }

    private void OnBlocksIconDestroy(List<Block> destroyBlockList)
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
                    GameController.GC.AddTimedAction(() => { OnBlocksFinishDestroy(destroyBlockList); }, 0.2f);
                };
            }
        }
    }

    private void OnBlocksFinishDestroy(List<Block> destroyBlockList)
    {
        // var colRowList = new Dictionary<float, float>();

        // foreach(var blockToDestroy in destroyBlockList)
        // {
        //     var colExists = colRowList.ContainsKey(blockToDestroy.BoardLoc.x);
        //     if(!colExists || colRowList[blockToDestroy.BoardLoc.x] < blockToDestroy.BoardLoc.y)
        //     {
        //         if(colExists) {
        //             colRowList.Remove(blockToDestroy.BoardLoc.x);
        //         }
        //         colRowList.Add(blockToDestroy.BoardLoc.x, blockToDestroy.BoardLoc.y);
        //     } 

        //     BlockList.Remove(blockToDestroy.BoardLoc);
        //     blockToDestroy.gameObject.SetActive(false);
        // }

        var fallLocList = new List<Vector2>();

        foreach(var blockToDestroy in destroyBlockList)
        {
            if(!fallLocList.Exists(loc => loc.x == blockToDestroy.BoardLoc.x && Mathf.Abs(loc.y - blockToDestroy.BoardLoc.y) <= 1))
            {
                fallLocList.Add(blockToDestroy.BoardLoc);
            }

            BlockList.Remove(blockToDestroy.BoardLoc);
            blockToDestroy.gameObject.SetActive(false);
        }


        //Decrement BlockDestroy Count
        BlockDestroyCount -= destroyBlockList.Count;

        //Drop blocks above the destroyed blocks
        foreach(var fallLoc in fallLocList)
        {
            var lockedBlock = LockBlockAboveLoc(fallLoc);
            GameController.GC.AddTimedAction(() => { 
                if(lockedBlock != null) {
                    lockedBlock.RemoveFallLock();
                }
                DropBlocksAboveLoc(fallLoc); 
            }, BlockFallDelay);    
        }

        // foreach(var col in colRowList.Keys)
        // { 
        //     var row = colRowList[col];
        //     var checkLoc = new Vector2(col, row);

        //     var lockedBlock = LockBlockAboveLoc(checkLoc);
        //     GameController.GC.AddTimedAction(() => { 
        //         if(lockedBlock != null) {
        //             lockedBlock.RemoveFallLock();
        //         }
        //         DropBlocksAboveLoc(checkLoc); 
        //     }, BlockFallDelay);
        // }
    }

    private Block LockBlockAboveLoc(Vector2 blockLoc)
    {
        Block blockToLock;
        BlockList.TryGetValue(blockLoc + Vector2.up, out blockToLock);
        
        if(blockToLock == null || blockToLock.IsDestroying) { 
            return null; 
        }
        
        blockToLock.IsMoveable = false;
        BlockList[blockLoc] = blockToLock;

        return blockToLock;
    }

    private void DropBlocksAboveLoc(Vector2 boardLoc, bool canBeFalling = false)
    {
        var blockListsToFall = GetBlocksAboveLoc(boardLoc);

        foreach(var blockToFallList in blockListsToFall) 
        {
            if(blockToFallList.Count > 0) { OnBlockStartFall(blockToFallList); }
        }
    }

    //Returns consecutive blocks above a location (spaces are separated in individual lists)
    private List<List<Block>> GetBlocksAboveLoc(Vector2 boardLoc, bool onlyFirstRow = false, bool canBeFalling = false)
    {
        var maxRow = onlyFirstRow ? boardLoc.y + 1 : BoardSize.y;
        var currentList = new List<Block>();
        var blocksAboveLists = new List<List<Block>>(){ currentList };
    
        for(var row = boardLoc.y; row < maxRow; row++)
        {
            boardLoc += Vector2.up;

            if(!BlockList.ContainsKey(boardLoc)) { 
                currentList = new List<Block>();
                blocksAboveLists.Add(currentList);
                continue;
            }

            if(!BlockList[boardLoc].IsFalling || canBeFalling) {
                currentList.Add(BlockList[boardLoc]);
            }
        }

        return blocksAboveLists;
    }
    private void OnBlockStartFall(List<Block> fallingBlockList)
    {
        var leadFallBlock = fallingBlockList.First();

        //Cancel the fall if there is now a block underneath that is not the lead block's "intent"
        var leadOpenLoc = new Vector2(leadFallBlock.BoardLoc.x, leadFallBlock.BoardLoc.y - 1);
        if(leadFallBlock.IsDestroying || leadFallBlock.IsFalling || 
            (BlockList.ContainsKey(leadOpenLoc) && BlockList[leadOpenLoc].GetInstanceID() != leadFallBlock.GetInstanceID())) { 
            return;
        }

        var linkedBlocks = fallingBlockList.Count > 1 ? fallingBlockList.Skip(1).ToList() : null;
        var removeFlag = false;

        foreach(var fallBlock in fallingBlockList) 
        {
            if(fallBlock.IsDestroying || fallBlock.IsFalling) { removeFlag = true; }
            if(removeFlag) { 
                linkedBlocks.Remove(fallBlock); 
            } else {
                var nextPos = new Vector2(fallBlock.BoardLoc.x, fallBlock.BoardLoc.y - 1);
                if(BlockList.ContainsKey(nextPos)) {
                    BlockList.Remove(nextPos);
                }
                BlockList.Add(nextPos, fallBlock);
            }
        }

        leadFallBlock.StartFall(true, linkedBlocks, () => { OnBlocksFinishMove(fallingBlockList, true); }); 
    }

    private bool CheckSurroundingBlocks(Block block, IEnumerable<Vector2> searchDirs, ref List<Block> matchingList, bool ignoreState = false)
    {
        var matchingLoc = new Vector2(block.BoardLoc.x, block.BoardLoc.y);

        var xMatchList = new List<Block>();
        var yMatchList = new List<Block>();

        foreach(var searchDir in searchDirs)
        {
            var searchLoc = searchDir;
            var depth = 2;

            while(depth > 0) {
                Block blockToCheck;

                if(BlockList.TryGetValue(matchingLoc + searchLoc, out blockToCheck) && blockToCheck.IsMatch(block, ignoreState))
                {
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

            if(!IsManuallyRaising && BlockDestroyCount <= 0) {  
                IsManuallyRaising = true;
                Speed = ManualRaiseSpeed;
            } 
        } else if(!performed && IsHoldingTrigger) {
            IsHoldingTrigger = false;
        }
    }

    private void UnlockTrigger()
    {
        if(IsHoldingTrigger) {
            Speed = ManualRaiseSpeed;
        } else {
            IsManuallyRaising = false;
        }
    }
}