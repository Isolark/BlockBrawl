using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType Type;
    private SpriteRenderer BlockSprite;
    public static SpriteManager BlockSM;
    public Vector3 BoardLoc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(BlockType type, Vector2 location)
    {
        Type = type;
        BoardLoc = location;

        GetComponent<SpriteRenderer>().sprite = BlockSM.SpriteList.Find(x => x.name == "Block-" + type.ToString());;
    }

    public void Move(Vector3 moveVector)
    {
        BoardLoc += moveVector;

        moveVector.Scale(new Vector3(GameController.GC.BlockDist, GameController.GC.BlockDist, 0));
        transform.localPosition += moveVector;
    }
}
