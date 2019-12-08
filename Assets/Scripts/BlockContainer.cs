using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    public ObjectPooler BlockPooler;
    public Vector2 Bounds;
    public int Types;
    public int StartingHeight;
    public float BaseSpeed;
    public float Speed;

    public bool AtTop;
    public float InitialBlock_Y;
    private float MinBlockY => BlockList.Select(x => x.transform.position.y).Min();
    public IList<Block> BlockList;

    public BlockContainer(int types, int startingHeight, float startingSpeed)
    {
        Types = types;
        StartingHeight = startingHeight;
        BaseSpeed = Speed = startingSpeed;
    }

    void Awake()
    {
        AtTop = false;
        InitialBlock_Y = -0.5f;
        BlockPooler = GameObject.Find("BlockPooler").GetComponent<ObjectPooler>();
        Block.BlockSM = GameObject.Find("BlockSM").GetComponent<SpriteManager>();
    }

    public void Initialize(Vector2 bounds)
    {
        Bounds = bounds;
        SpawnRows(StartingHeight + 1, rowModVals: new List<int>(){-1, 0, 0, 1});
    }

    public void SpawnRows(int numOfRows = 1, int numOfCols = 6, IList<int> rowModVals = null)
    {
        //var 
        for(var col = 0; col < numOfCols; col++)
        {
            var modRows = numOfRows;
            if(rowModVals != null)
            {
                modRows += rowModVals[Random.Range(0, rowModVals.Count-1)];
            }

            for(var row = 0; row < modRows; row++)
            {
                var block = BlockPooler.GetPooledObject(transform).GetComponent<Block>();
                var type = (BlockType)Random.Range(1, Types + 1);

                block.Initialize(type);
                Debug.Log(InitialBlock_Y);
                block.gameObject.TransBySpriteDimensions(new Vector3(col - 2.5f, InitialBlock_Y + row, 0));
            }
        }
    }

    public void Move()
    {

    }
}