using System.Collections.Generic;
using UnityEngine;

//Consider only growing after hitting Limit multiple times
//Else, just create one that will eventually be completely destroyed
//Prevents having too large of a list
public abstract class ObjectPooler : MonoBehaviour 
{
    public GameObject pooledObj;
    public int poolMaxSize = 20;
    public bool expandable = true;
    IList<GameObject> pool;

    protected virtual void Awake()
    {
        pool = new List<GameObject>();
        for(var x = 0; x < poolMaxSize; x++)
        {
            var obj = Instantiate(pooledObj);
            obj.name = obj.name.Replace("(Clone)", string.Empty);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public virtual GameObject GetPooledObject(string objName, Transform parent = null)
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
        
        obj.name = objName;

        return obj;
    }

    public virtual void RepoolObject(GameObject obj)
    {
    }

    public void RepoolAllObjects()
    {
        foreach(var obj in pool)
        {
            RepoolObject(obj);
            obj.transform.SetParent(gameObject.transform);
            obj.SetActive(false);
        }
    }
}