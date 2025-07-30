using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool : MonoBehaviour
{
    public bool activate;

    public string objectPoolName;
    public GameObject targetPrefab;
    public int extraCount;

    private List<GameObject> objects;
    private List<bool> objectUse;

    public void InitSetting(string name, GameObject prefab, int count)
    {
        objectPoolName = name;
        targetPrefab = prefab;
        extraCount = count;
        objects = new List<GameObject>();
        objectUse = new List<bool>();
        activate = true;
        RemainObjectCheck();
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
