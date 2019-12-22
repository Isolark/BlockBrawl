using UnityEngine;

public class SpriteFXPooler : ObjectPooler
{
    public static SpriteFXPooler SP;

    override protected void Awake()
    {
        //Singleton pattern
        if (SP == null) {
            if(transform.parent.gameObject) {
                DontDestroyOnLoad(transform.parent.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }

            SP = this;
        }
        else if (SP != this) {
            Destroy(gameObject);
        }
        
        base.Awake();
    }

    public GameObject GetPooledObject(string layerName, bool destroySelf = true, Transform parent = null)
    {
        var spriteFX = base.GetPooledObject(parent).GetComponent<SpriteFX>();
        spriteFX.DestroySelf = destroySelf;

        if(layerName != null) {
            spriteFX.FXSprite.sortingLayerName = layerName;
        } 

        return spriteFX.gameObject;
    }
}