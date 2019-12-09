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
            if(transform.parent.gameObject) {
                DontDestroyOnLoad(transform.parent.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }

            OP = this;
        }
        else if (OP != this) {
            Destroy(gameObject);
        }

        pool = new List<GameObject>();
        for(var x = 0; x < poolMaxSize; x++)
        {
            var obj = Instantiate(pooledObj);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject(Transform parent = null)
    {
        GameObject obj = null;

        foreach(var tempObj in pool)
        {
            if(!tempObj.activeSelf) { 
                obj = tempObj;
                break;
            }
        }

        if(obj == null && expandable) 
        {
            obj = Instantiate(pooledObj);
            pool.Add(obj);
        }

        if(obj != null) {
            obj.SetActive(true);

            if(parent != null) { 
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector2.zero;
            }
        }

        return obj;
    }
}