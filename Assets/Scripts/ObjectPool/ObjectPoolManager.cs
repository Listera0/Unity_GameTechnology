using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolLinker
{
    [Tooltip("Setting activate pool")]
    public bool activate;
    [Tooltip("ObjectPool's name")]
    public string objectPoolName;
    [Tooltip("Target Object's Prefab")]
    public GameObject targetPrefab;
    [Tooltip("How many create extra objects")]
    public int extraCount;
}

[DefaultExecutionOrder(-1001)]
public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager Instance;
    public static ObjectPoolManager instance { get { return Instance; } }

    public List<ObjectPoolLinker> objectPoolLinker;

    private Dictionary<string, ObjectPool> objectPools;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(FindRootParent());
    }

    private void Start()
    {
        objectPools = new Dictionary<string, ObjectPool>();

        foreach (ObjectPoolLinker linker in objectPoolLinker)
        {
            if (linker.activate == false || CheckObjectPool(linker) == false) continue;

            GameObject objectPoolObj = new GameObject(string.Format("{0} ObjectPool", linker.objectPoolName));
            objectPoolObj.transform.SetParent(transform);
            ObjectPool newObjectPool = objectPoolObj.AddComponent<ObjectPool>();
            newObjectPool.InitSetting(linker.objectPoolName, linker.targetPrefab, linker.extraCount);
            objectPools.Add(linker.objectPoolName, newObjectPool);
        }
    }

    private GameObject FindRootParent()
    {
        GameObject parentObj = gameObject;

        while (parentObj.transform.parent != null)
        {
            parentObj = parentObj.transform.parent.gameObject;
        }

        return parentObj;
    }

    public bool CheckObjectPool(ObjectPoolLinker linker)
    {
        bool checkResult = true;

        if (linker.objectPoolName == "")
        {
            Debug.LogWarning("ObjectPoolName is Null");
            return false;
        }

        if (objectPools.ContainsKey(linker.objectPoolName) == true)
        {
            Debug.LogWarningFormat("Same name of objectpool [{0}]", linker.objectPoolName);
            checkResult = false;
        }

        if (linker.targetPrefab == null)
        {
            Debug.LogWarningFormat("{0} TargetPrefab is Null", linker.objectPoolName);
            checkResult = false;
        }

        if (linker.extraCount < 1)
        {
            Debug.LogWarningFormat("{0} ExtraCount is lower then 1", linker.objectPoolName);
            checkResult = false;
        }

        return checkResult;
    }

    public GameObject GetObjectFromPool(string poolName)
    {
        if (objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject();
        }
        return null;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.GetComponent<PoolObject>().ownerPool.ReturnObject(obj);
    }
}
