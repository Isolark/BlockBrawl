using System.Collections.Generic;
using UnityEngine;

//Consider only growing after hitting Limit multiple times
//Else, just create one that will eventually be completely destroyed
//Prevents having too large of a list
public class BlockPooler : ObjectPooler 
{
    public static BlockPooler BP;
    void Awake()
    {
        //Singleton pattern
        if (BP == null) {
            if(transform.parent.gameObject) {
                DontDestroyOnLoad(transform.parent.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }

            BP = this;
        }
        else if (BP != this) {
            Destroy(gameObject);
        }
    }

    public GameObject GetPooledBlock(BlockType type)
    {
        var obj = GetPooledObject();

        if(obj) {
            var block = obj.GetComponent<Block>();
            block.Initialize(type);
        }

        return obj;
    }
}