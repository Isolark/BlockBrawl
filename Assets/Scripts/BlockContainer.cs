using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    public ObjectPooler BlockPooler;
    public GameCursor Cursor;
    public Vector2 BoardSize;
    public int Types;
    public int StartingHeight;
    public float Speed;
    public float BaseSpeed;
    public float ManualRaiseSpeed;

    private bool IsHoldingTrigger;
    private bool IsManuallyRaising;
    private float BlockDist;

    public bool AtTop;
    public float InitialBlock_Y;
    public float Target_Y; //After reaching this Y, have moved up a full "level"
    public IDictionary<Vector2, Block> BlockList;
    private IDictionary<Vector2, Block> TmpBlockList = new Dictionary<Vector2, Block>(); //For transferring after moving up a "level"

    public BlockContainer(int types, int startingHeight, float startingSpeed)
    {
        Types = types;
        StartingHeight = startingHeight;
        BaseSpeed = Speed = startingSpeed;
    }

    void Awake()
    {
        AtTop = false;
        BlockList = new Dictionary<Vector2, Block>();

        //BlockPooler = GameObject.Find("BlockPooler").GetComponent<ObjectPooler>();
        //Cursor = GameObject.Find("GameCursor").GetComponent<GameCursor>();
        BlockExtensions.BlockSL = GameObject.Find("BlockSL").GetComponent<SpriteLibrary>();
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

        SpawnRows(StartingHeight + 1, rowModVals: new List<int>(){-1, 0, 0, 1});
    }

    public void SpawnRows(int numOfRows = 1, int numOfCols = 6, int startingRow = 0, IList<int> rowModVals = null)
    {
        for(var col = 0; col < numOfCols; col++)
        {
            var modRows = numOfRows;
            if(rowModVals != null)
            {
                modRows += rowModVals[Random.Range(0, rowModVals.Count-1)];
            }

            for(var row = startingRow; row < modRows; row++)
            {
                var block = BlockPooler.GetPooledObject(transform).GetComponent<Block>();
                var type = (BlockType)Random.Range(1, Types + 1);
                var loc = new Vector2(col, row);

                BlockList.Add(loc, block);
                block.Initialize(type, loc, (row > 0 && row <= 12));
                block.gameObject.TransBySpriteDimensions(new Vector3(col - 2.5f, InitialBlock_Y + row, 0));
            }
        }
    }

    public void Move()
    {
        if(!AtTop)
        {
            var yPos = transform.localPosition.y;

            if(yPos < Target_Y) 
            {
                //Reached next "level". Shift down & move all blocks up
                if(yPos + Speed > Target_Y)
                {
                    transform.localPosition.Set(transform.localPosition.x, Target_Y, 1);
                    Target_Y++;

                    //Shift container down
                    //transform.localPosition = new Vector3(0, Target_Y - BlockDist, 0);

                    //Shift blocks up
                    TmpBlockList = new Dictionary<Vector2, Block>();
                    foreach(var block in BlockList)
                    {
                        if(block.Value.HasIterated) continue;

                        block.Value.HasIterated = true;

                        var nextKey = block.Key + Vector2.up;
                        
                        if(nextKey.y >= BoardSize.y)
                        {
                            AtTop = Cursor.AtTop = true;
                        }

                        TmpBlockList.Add(nextKey, block.Value);
                        //block.Value.InstantMove(Vector2.up);
                        block.Value.MoveBoardLoc(Vector2.up, true);
                        block.Value.OnEnterBoard();
                    }

                    BlockList = TmpBlockList;

                    foreach(var block in BlockList) {
                        block.Value.HasIterated = false;
                    }

                    InitialBlock_Y--;

                    SpawnRows();

                    //Shift cursor up
                    Cursor.MoveBoardLoc(Vector2.up);
                    if(!AtTop && Cursor.BoardLoc.y >= BoardSize.y) {
                        Cursor.OnMove(Vector2.down);
                    }
                    //Cursor.OnMove(Vector2.up);
                    
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

    private void UnlockTrigger()
    {
        if(IsHoldingTrigger) {
            Speed = ManualRaiseSpeed;
        } else {
            IsManuallyRaising = false;
        }
    }

    public void OnCursorConfirm(Vector2 cursorLoc)
    {
        Block leftBlock, rightBlock;

        //Find Blocks. If either immobile, return
        var leftBlockExists = BlockList.TryGetValue(cursorLoc, out leftBlock);
        if(leftBlockExists && !leftBlock.IsMoveable) return;

        var rightBlockExists = BlockList.TryGetValue(cursorLoc + Vector2.right, out rightBlock);
        if(rightBlockExists && !rightBlock.IsMoveable) return;
        
        //Move Blocks if they exist
        if(leftBlockExists) {
            if(!rightBlockExists) {
                leftBlock.Move(Vector2.right, callback: () => { BlockList.Remove(leftBlock.PrevBoardLoc); });
            } else {
                leftBlock.Move(Vector2.right);
            }

            BlockList[leftBlock.BoardLoc] = leftBlock;
        }
        if(rightBlockExists) {
            if(!leftBlockExists) {
                rightBlock.Move(Vector2.left, callback: () => { BlockList.Remove(rightBlock.PrevBoardLoc); });
            } else {
                rightBlock.Move(Vector2.left, bumpOrder: true);
            }

            BlockList[rightBlock.BoardLoc] = rightBlock;
        }
    }

    //Start or Stop manual speed increase
    public void OnTrigger(bool performed)
    {
        if(performed && !IsHoldingTrigger) {
            IsHoldingTrigger = true;

            if(!IsManuallyRaising) {  
                IsManuallyRaising = true;
                Speed = ManualRaiseSpeed;
            } 
        } else if(!performed && IsHoldingTrigger) {
            IsHoldingTrigger = false;
        }
    }
}