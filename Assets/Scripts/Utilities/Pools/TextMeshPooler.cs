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

    public GameObject GetPooledObject(string storedTextName, Transform parent = null)
    {
        var obj = GetPooledObject(parent);
        var objText = obj.GetComponent<TMP_Text>();
        var storedText = GetStoredTextByName(storedTextName);
        
        objText.fontSize = storedText.FontSize;
        objText.color = storedText.VertexColor;

        return obj;
    }

    public TextMeshSO GetStoredTextByName(string storedTextName)
    {
        return StoredTexts.Find(x => x.name == storedTextName);
    } 
}