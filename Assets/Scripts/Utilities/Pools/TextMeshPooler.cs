using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TextMeshPooler : ObjectPooler
{
    public List<TextMeshSO> StoredTexts;
    public List<TMP_FontAsset> StoredFonts;
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

    public GameObject GetPooledObject(string storedTextName, Vector2 rectSize, string layerName, 
        int layerOrder = 0, string objName = "GameText", Transform parent = null)
    {
        var obj = base.GetPooledObject(objName, parent);
        var objText = obj.GetComponent<TMP_Text>();
        var objMesh = obj.GetComponent<MeshRenderer>();
        var storedText = GetStoredTextByName(storedTextName);
        
        if(!string.IsNullOrWhiteSpace(layerName)) { objMesh.sortingLayerName = layerName; } 

        objText.font = storedText.FontAsset;
        objMesh.sortingOrder = layerOrder;
        objText.rectTransform.sizeDelta = rectSize;
        objText.fontSize = storedText.FontSize;
        objText.color = storedText.VertexColor;
        objText.fontStyle = storedText.IsBold ? FontStyles.Bold : FontStyles.Normal;
        objText.margin = Vector4.zero;

        return obj;
    }

    public TextMeshSO GetStoredTextByName(string storedTextName)
    {
        return StoredTexts.Find(x => x.name == storedTextName);
    } 

    public TMP_FontAsset GetStoredFontByName(string fontName)
    {
        return StoredFonts.Find(x => x.name.Contains(fontName));
    }

    override public void RepoolObject(GameObject obj)
    {
        var textMeshText = obj.GetComponent<TMP_Text>();
        RepoolTextMeshText(textMeshText);
    }

    public void RepoolTextMeshText(TMP_Text textMeshText)
    {
        textMeshText.gameObject.transform.ResetTransform(true);
        textMeshText.gameObject.SetActive(false);
    }
}