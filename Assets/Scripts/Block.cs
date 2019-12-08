using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType Type;
    private SpriteRenderer BlockSprite;
    public static SpriteManager BlockSM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(BlockType type)
    {
        Type = type;
        var sprite = BlockSM.SpriteList.Find(x => x.name == "Block-" + type.ToString());
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
