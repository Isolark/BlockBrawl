using UnityEngine;
using System.Collections.Generic;

//Library for holding ScriptableObjects
public class DataLibrary : MonoBehaviour
{
    public List<ScriptableObject> DataList;

    public static DataLibrary DL;

    void Awake()
    {
        //Singleton pattern
        if (DL == null) {
            DL = this;
        }
        else if (DL != this) {
            Destroy(gameObject);
        }     
    }

    public ScriptableObject GetDataByName(string name)
    {
        return DataList.Find(x => x.name == name);
    }
}