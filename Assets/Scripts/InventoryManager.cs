using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInfo
{
    public ItemData[,] inventoryItemData;

    public InventoryInfo(int sizeX, int sizeY)
    {
        inventoryItemData = new ItemData[sizeX, sizeY];
    }
}

public interface IInventorySystem
{
    void GetItem(ItemData item);
    void AddItem(ItemData item, int index);
    void RemoveItem(ItemData item);
    void RemoveItemFromSlot(int index, int count);

    int[] FindItemIndex(ItemData item);
    int[] FindEmptySlot(ItemData item);

    int[] GetInventoryIndex(int index);
}

public class InventoryManager : Singleton<InventoryManager>, IInitializeInter
{
    public GameObject dragSlot;
    public ItemData movingItemData;

    public void Initialize()
    {
        if (!dragSlot) Debug.LogWarning("dragSlot is Null");
    }
}
