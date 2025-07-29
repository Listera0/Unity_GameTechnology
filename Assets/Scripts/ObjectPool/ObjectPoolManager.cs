using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private List<ObjectPool> objectPools;
    private List<string> objectPoolNames;

    void Start()
    {
        objectPools = new List<ObjectPool>();
        objectPoolNames = new List<string>();

        foreach (Transform child in transform)
        {
            ObjectPool childObjectPool = child.GetComponent<ObjectPool>();

            if (childObjectPool == null)
            {
                Debug.LogWarningFormat("[{0}] has not 'ObjectPool' Component", child.gameObject.name);
                continue;
            }

            if (objectPoolNames.Contains(childObjectPool.objectPoolName) == true)
            {
                Debug.LogWarningFormat("[{0}] has same name", child.gameObject.name);
                continue;
            }

            if (childObjectPool.CheckObjectPool() == false)
            {
                continue;
            }

            childObjectPool.InitSetting();
            objectPools.Add(childObjectPool);
            objectPoolNames.Add(childObjectPool.objectPoolName);
        }
    }

    public GameObject GetObjectFromPool(string poolName)
    {
        for (int i = 0; i < objectPoolNames.Count; i++)
        {
            if (objectPoolNames[i] == poolName)
            {
                return objectPools[i].GetObject();
            }
        }
        return null;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.GetComponent<PoolObject>().ownerPool.ReturnObject(obj);
    }
}
