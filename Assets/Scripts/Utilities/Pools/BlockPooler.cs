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
        var block = base.GetPooledObject("Block", parent).GetComponent<Block>();
        block.Reset();

        return block.gameObject;
    }

    public void RepoolObject(Block block)
    {
        block.Reset();
        block.gameObject.SetActive(false);
    }
}