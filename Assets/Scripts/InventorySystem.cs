using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>, IInitializeInter
{
    public GameObject dragSlot;
    public ItemData movingItemData;

    public void Initialize()
    {
        if (!dragSlot) Debug.LogWarning("dragSlot is Null");
    }
}
