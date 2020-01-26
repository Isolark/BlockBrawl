using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    override public void RepoolObject(GameObject obj)
    {
        var spriteFX = obj.GetComponent<SpriteFX>();
        RepoolSpriteFX(spriteFX);
    }

    public void RepoolSpriteFX(SpriteFX spriteFX)
    {
        if(!spriteFX.gameObject.activeSelf) { return; }
        
        var childObjList = new List<Transform>();
        spriteFX.transform.GetAllChildrenRecursively(ref childObjList);

        foreach(var childObj in childObjList)
        {
            SpriteFX sprFX;
            if(childObj.TryGetComponent<SpriteFX>(out sprFX))
            {
                sprFX.Reset();
                sprFX.gameObject.SetActive(false);
                continue;
            }

            TMP_Text tmpText;
            if(childObj.TryGetComponent<TMP_Text>(out tmpText))
            {
                TextMeshPooler.TMP.RepoolTextMeshText(tmpText);
                continue;
            }

            childObj.ResetTransform(true);
        }

        // foreach(var childSpriteFX in spriteFX.GetComponentsInChildren<SpriteFX>().Where(x => x.GetInstanceID() != spriteFX.GetInstanceID()))
        // {
        //     RepoolSpriteFX(childSpriteFX);
        // }
        // foreach(var textMeshText in spriteFX.GetComponentsInChildren<TMP_Text>())
        // {
        //     Debug.Log("here");
        //     TextMeshPooler.TMP.RepoolTextMeshText(textMeshText);
        // }

        spriteFX.Reset();
        spriteFX.gameObject.SetActive(false);
    }
}