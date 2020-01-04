using UnityEngine;

public class BlockPooler : ObjectPooler
{
    public static BlockPooler BP;

    override protected void Awake()
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

        base.Awake();
    }

    public GameObject GetPooledObject(Transform parent = null)
    {
        return base.GetPooledObject("Block", parent);
    }

    override protected void CleanObj(ref GameObject obj)
    {
        var block = obj.GetComponent<Block>();
        block.BlockSprite.color = Color.white;
        block.StoredAction = null;
    }
}