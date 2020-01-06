using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TextMeshPooler : ObjectPooler
{
    public List<TextMeshSO> StoredTexts;
    public static TextMeshPooler TMP;

    override protected void Awake()
    {
        //Singleton pattern
        if (TMP == null) {
            if(transform.parent.gameObject) {
                DontDestroyOnLoad(transform.parent.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }

            TMP = this;
        }
        else if (TMP != this) {
            Destroy(gameObject);
        }

        base.Awake();
    }

    public GameObject GetPooledObject(string storedTextName, string layerName, int layerOrder = 0, string objName = "GameText", Transform parent = null)
    {
        var obj = base.GetPooledObject(objName, parent);
        var objText = obj.GetComponent<TMP_Text>();
        var objMesh = obj.GetComponent<MeshRenderer>();
        var storedText = GetStoredTextByName(storedTextName);
        
        if(!string.IsNullOrWhiteSpace(layerName)) { objMesh.sortingLayerName = layerName; } 

        objMesh.sortingOrder = layerOrder;
        objText.fontSize = storedText.FontSize;
        objText.color = storedText.VertexColor;
        objText.fontStyle = storedText.IsBold ? FontStyles.Bold : FontStyles.Normal;

        return obj;
    }

    public TextMeshSO GetStoredTextByName(string storedTextName)
    {
        return StoredTexts.Find(x => x.name == storedTextName);
    } 
}