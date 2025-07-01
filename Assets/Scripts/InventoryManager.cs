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
    int[] FindItemIndex(ItemData item, bool includeMaxStack);
    // Find Empty Slot
    int[] FindEmptySlot();
    // translate index to inventory slot index(int -> int[2])
    int[] GetInventoryIndex(int index);
}

public class InventoryManager : Singleton<InventoryManager>, IInitializeInter
{
    public GameObject dragSlot;
    public int dragIndex;
    public ItemData movingItemData;

    public void Initialize()
    {
        if (!dragSlot) Debug.LogWarning("dragSlot is Null");
    }
}
