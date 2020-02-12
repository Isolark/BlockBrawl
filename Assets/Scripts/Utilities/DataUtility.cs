using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System;

public static class DataUtility
{
    //private static readonly string X = "S2FOZFJnVWtYcDJzNXY4eS9CP0UoSCtNYlFlU2hWbVk=";
    private static readonly string X = "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";
    private static readonly int KEY_SIZE = 256;
    private static readonly int IV_SIZE = 16; // block size is 128-bit

    public static bool CheckFileExists(string path)
    {
        var absPath = Application.persistentDataPath + path;
        return File.Exists(absPath);
    }

    public static string LoadStringFromTxt(string path)
    {
        var absPath = Application.persistentDataPath + path;

        if(File.Exists(absPath)) {
            return File.ReadAllText(absPath);
        } else {
            Debug.Log("Error! Could not load JSON from path: " + absPath);
            return null;
        }
    }

    public static T LoadFromJSON<T>(string path)
    {
        var absPath = Application.persistentDataPath + path;
        Debug.Log(absPath);
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
        var key = Convert.FromBase64String(X);

        if(File.Exists(absPath)) 
        {
            T loadedObj;

            using(var fs = new FileStream(absPath, FileMode.Open))
            {
                using(var cryptoStream = CreateDecryptionStream(key, fs))
                {
                    loadedObj = (T)ReadObjectFromStream(cryptoStream);
                }
            }

            return loadedObj;
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
        var key = Convert.FromBase64String(X);

        using(var fs = new FileStream(absPath, FileMode.OpenOrCreate))
        {
            using(var cryptoStream = CreateEncryptionStream(key, fs))
            {
                WriteObjectToStream(cryptoStream, dataObj);
            }
            fs.Close();
        }
    }

    //Binary Encryption: https://stackoverflow.com/questions/28791185/encrypt-net-binary-serialization-stream
    public static void WriteObjectToStream(Stream outputStream, System.Object obj)
    {
        if (object.ReferenceEquals(null, obj))
        {
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(outputStream, obj);
    }

    public static object ReadObjectFromStream(Stream inputStream)
    {
        BinaryFormatter binForm = new BinaryFormatter();
        object obj = binForm.Deserialize(inputStream);
        return obj;
    }

    public static CryptoStream CreateEncryptionStream(byte[] key, Stream outputStream)
    {
        byte[] iv = new byte[IV_SIZE];

        using (var rng = new RNGCryptoServiceProvider())
        {
            // Using a cryptographic random number generator
            rng.GetNonZeroBytes(iv);
        }

        // Write IV to the start of the stream
        outputStream.Write(iv, 0, iv.Length);

        Rijndael rijndael = new RijndaelManaged();
        rijndael.KeySize = KEY_SIZE;

        CryptoStream encryptor = new CryptoStream(
            outputStream,
            rijndael.CreateEncryptor(key, iv),
            CryptoStreamMode.Write);
        return encryptor;
    }

    public static CryptoStream CreateDecryptionStream(byte[] key, Stream inputStream)
    {
        byte[] iv = new byte[IV_SIZE];

        if (inputStream.Read(iv, 0, iv.Length) != iv.Length)
        {
            throw new ApplicationException("Failed to read IV from stream.");
        }

        Rijndael rijndael = new RijndaelManaged();
        rijndael.KeySize = KEY_SIZE;

        CryptoStream decryptor = new CryptoStream(
            inputStream,
            rijndael.CreateDecryptor(key, iv),
            CryptoStreamMode.Read);
        return decryptor;
    }
}