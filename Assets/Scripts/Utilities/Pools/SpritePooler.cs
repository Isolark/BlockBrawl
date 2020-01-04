using UnityEngine;

public class SpritePooler : ObjectPooler
{
    public static SpritePooler SP;

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

    public GameObject GetPooledObject(string spriteName, string layerName, Transform parent = null)
    {
        var spriteRenderer = base.GetPooledObject("Sprite", parent).GetComponent<SpriteRenderer>();
        
        if(!string.IsNullOrWhiteSpace(spriteName)) { spriteRenderer.sprite = SpriteLibrary.SL.GetSpriteByName(spriteName); }
        if(!string.IsNullOrWhiteSpace(layerName)) { spriteRenderer.sortingLayerName = layerName; }
        
        return spriteRenderer.gameObject;
    }
}