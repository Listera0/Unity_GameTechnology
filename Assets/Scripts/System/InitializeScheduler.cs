using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializeInter
{
    void Initialize();
}

[DefaultExecutionOrder(-1000)]
public class InitializeScheduler : MonoBehaviour
{
    [SerializeField]
    private List<MonoBehaviour> initializeClasses;

    void Awake()
    {
        foreach (var script in initializeClasses)
        {
            if (script is IInitializeInter initialize)
            {
                initialize.Initialize();
            }
        }
    }
}
