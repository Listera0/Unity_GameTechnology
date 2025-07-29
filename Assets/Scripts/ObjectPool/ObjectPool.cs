using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public string objectPoolName;
    
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int extraCount;
    [SerializeField] private bool activate;

    private List<GameObject> objects;
    private List<bool> objectUse;

    public void InitSetting()
    {
        objects = new List<GameObject>();
        objectUse = new List<bool>();
        activate = true;
        RemainObjectCheck();
    }

    public bool CheckObjectPool()
    {
        bool checkResult = true;

        if (objectPoolName == "")
        {
            Debug.LogWarning("ObjectPoolName is Null");
            checkResult = false;
        }

        if (targetPrefab == null)
        {
            Debug.LogWarning("TargetPrefab is Null");
            checkResult = false;
        }

        if (extraCount < 1)
        {
            Debug.LogWarning("ExtraCount is lower then 1");
            checkResult = false;
        }

        return checkResult;
    }

    private void RemainObjectCheck()
    {
        if (activate == false) return;

        int maintainCount = 0;
        foreach (bool use in objectUse)
        {
            if (use == false) maintainCount++;
        }

        for (int i = 0; i < extraCount - maintainCount; i++)
        {
            GameObject newObj = Instantiate(targetPrefab, transform);
            newObj.SetActive(false);
            PoolObject poolObj = newObj.AddComponent<PoolObject>();
            poolObj.ownerPool = this;

            objects.Add(newObj);
            objectUse.Add(false);
        }
    }

    public GameObject GetObject()
    {
        if (activate == false) return null;
        RemainObjectCheck();

        for (int i = 0; i < objectUse.Count; i++)
        {
            if (objectUse[i] == false)
            {
                objectUse[i] = true;
                objects[i].SetActive(true);
                return objects[i];
            }
        }
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        if (activate == false) return;

        int index = objects.IndexOf(obj);
        if (index != -1)
        {
            objectUse[index] = false;
            obj.transform.SetParent(transform);
            obj.SetActive(false);
        }
    }
}
