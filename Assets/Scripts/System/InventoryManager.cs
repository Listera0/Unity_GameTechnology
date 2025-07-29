using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryCategory
{
    Single = 0,
    Classification,
    Allocation,
    Extended
}

public class InventoryInfo
{
    public InventoryCategory inventoryCategory;
    public ItemData[] inventoryItemData;
    public int[] inventoryItemLink;

    public InventoryInfo(InventoryCategory category, int size)
    {
        inventoryCategory = category;
        inventoryItemData = new ItemData[size];
        inventoryItemLink = new int[size];
        Array.Fill(inventoryItemLink, -1);
    }
}

public interface IInventorySystem
{
    // return inventorycategory
    InventoryCategory GetInventoryCategory();
    // Only Add Item, Stacking if can Stack
    void GetItem(ItemData item);
    // Add Item With SlotIndex
    void AddItem(ItemData item, int index);
    // Only Remove Item Where it was
    void RemoveItem(ItemData item);
    // Remove Item From Slot with Count
    void RemoveItemFromSlot(int index, int count);
    // MoveItem include from, to index
    void MoveItemToSlot(int fromIndex, int toIndex);
    // Show Inventory Item to UI
    void ShowInventory();
    // find item Slot Index Optional includeMaxStack
    int FindItemIndex(ItemData item, bool includeMaxStack);
    // Find Empty Slot
    int FindEmptySlot();
}

public class InventoryManager : Singleton<InventoryManager>, IInitializeInter
{
    public GameObject dragSlot;
    public int dragIndex;
    public int dragOffset;
    public ItemData movingItemData;
    public Transform trashObject;

    public void Initialize()
    {
        if (!dragSlot) Debug.LogWarning("dragSlot is Null");
    }

    public void ClearTrash()
    {
        List<Transform> childs = new List<Transform>();

        foreach (Transform child in dragSlot.transform)
        {
            childs.Add(child);
        }

        foreach (Transform child in childs)
        {
            Destroy(child.gameObject);
        }
    }
}
