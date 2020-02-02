using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataUtility
{
    public static bool CheckExistsJSON(string path)
    {
        var absPath = Application.persistentDataPath + path;
        Debug.Log(absPath);
        return File.Exists(absPath);
    }

    public static T LoadFromJSON<T>(string path)
    {
        var absPath = Application.persistentDataPath + path;

        if(File.Exists(absPath)) {
            var jsonData = File.ReadAllText(absPath);
            return JsonUtility.FromJson<T>(jsonData);
        } else {
            Debug.Log("Error! Could not load JSON from path: " + absPath);
            return default(T);
        }
    }

    public static void SaveToJSON(string path, object dataObj)
    {
        var absPath = Application.persistentDataPath + path;
        var jsonData = JsonUtility.ToJson(dataObj);

        File.WriteAllText(absPath, jsonData);
    }

    public static T LoadFromBinary<T>(string path)
    {
        var absPath = Application.persistentDataPath + path;
        if(File.Exists(absPath)) 
        {
            var file = new FileStream(absPath, FileMode.Open);
            var formatter = new BinaryFormatter();
            var loadedObj = formatter.Deserialize(file);

            return (T)loadedObj;
        } 
        else 
        {
            Debug.Log("Error! Could not load Binary from path: " + absPath);
            return default(T);
        }
    }

    public static void SaveToBinary(string path, object dataObj)
    {
        var absPath = Application.persistentDataPath + path;
        var file = new FileStream(absPath, FileMode.OpenOrCreate);
        var formatter = new BinaryFormatter();

        formatter.Serialize(file, dataObj);
        file.Close();
    }
}