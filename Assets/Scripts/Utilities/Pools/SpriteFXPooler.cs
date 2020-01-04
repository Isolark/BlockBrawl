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

    public GameObject GetPooledObject(string objName = "SpriteFX", string layerName = "Default", int layerOrder = 0, bool destroySelf = true, Transform parent = null)
    {
        var spriteFX = base.GetPooledObject(objName, parent).GetComponent<SpriteFX>();
        
        if(!string.IsNullOrWhiteSpace(layerName)) { spriteFX.FXSprite.sortingLayerName = layerName; }

        spriteFX.FXSprite.sortingOrder = layerOrder;
        spriteFX.DestroySelf = destroySelf;

        return spriteFX.gameObject;
    }

    override protected void CleanObj(ref GameObject gameObj)
    {
        var spriteFX = gameObj.GetComponent<SpriteFX>();

        spriteFX.StateCallbacks.Clear();
        spriteFX.FXAnimCtrl.runtimeAnimatorController = null;
        spriteFX.FXAnimCtrl.enabled = false;
        spriteFX.FXSprite.color = Color.white;
        spriteFX.FXSprite.sprite = null;
    }
}