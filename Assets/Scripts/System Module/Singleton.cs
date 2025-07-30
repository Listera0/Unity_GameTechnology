using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1001)]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T Instance;

    public static T instance { get { return Instance; } }

    protected virtual void OnAwake() { }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = GetComponent<T>();
        DontDestroyOnLoad(FindRootParent());

        OnAwake();
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
}
