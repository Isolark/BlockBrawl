using System.Collections.Generic;
using UnityEngine;

//Consider only growing after hitting Limit multiple times
//Else, just create one that will eventually be completely destroyed
//Prevents having too large of a list
public class ObjectPooler : MonoBehaviour 
{
    public static ObjectPooler OP;

    public GameObject pooledObj;
    public int poolMaxSize = 20;
    public bool expandable = true;

    IList<GameObject> pool;

    void Awake()
    {
        //Singleton pattern
        if (OP == null) {
            DontDestroyOnLoad(gameObject);
            OP = this;
        }
        else if (OP != this) {
            Destroy(gameObject);
        }

        pool = new List<GameObject>();
        for(var x = 0; x < poolMaxSize; x++)
        {
            var obj = Instantiate(pooledObj);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach(var obj in pool)
        {
            if(!obj.activeSelf) { 
                obj.SetActive(true);
                return obj; 
            }
        }

        if(expandable) {
            var obj = Instantiate(pooledObj);
            obj.transform.SetParent(this.transform);
            pool.Add(obj);
            return obj;
        }

        return null;
    }
}