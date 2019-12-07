using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    private readonly int Types;
    public float BaseSpeed;
    public float Speed;
    public bool AtTop = false;
    public readonly float InitialBlock_Y = -0.5f;
    public IList<Block> BlockList;

    public BlockContainer(int types, float startingSpeed)
    {
        Types = types;
        BaseSpeed = Speed = startingSpeed;
    }

    void Start()
    {

    }

    public void SpawnRows(int numOfRows = 1)
    {
        for(var i = 0; i < numOfRows; i++)
        {
            var block = new Block((BlockType)Random.Range(1, Types));
        }
    }

    public void Move()
    {

    }
}