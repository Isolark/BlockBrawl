using System.Collections.Generic;
using UnityEngine;

//Holds Data (SOs) that lives between Scenes (e.g. Level Data, User Data, etc...)
public class DataManager : MonoBehaviour 
{
    public MainController MainCtrl;
    public Dictionary<string, ScriptableObject> DataList;

    void Awake()
    {
        DataList = new Dictionary<string, ScriptableObject>();
    }

    public void LoadData(string name, string path)
    {
        var loadedData = Resources.Load<ScriptableObject>(path);
        if(DataList.ContainsKey(name)) { DataList.Remove(name); }

        DataList.Add(name, loadedData);
    }
}