using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllocationInventory : IInventorySystem
{
    private InventoryInfo inventoryInfo;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(64);
    }

    public void GetItem(ItemData item)
    {
        
    }

    public void AddItem(ItemData item, int index)
    {

    }

    public void RemoveItem(ItemData item)
    {

    }

    public void RemoveItemFromSlot(int index, int count)
    {

    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {

    }

    public void ShowInventory()
    {

    }

    public int FindItemIndex(ItemData item, bool includeMaxStack)
    {
        return -1;
    }

    public int FindEmptySlot()
    {
        return -1;
    }

    // public int GetOringinItem(int index)
    // {
    //     int[] targetIndex = GetInventoryIndex(index);
    //     return inventoryInfo.inventoryItemLink[targetIndex[0], targetIndex[1]];
    // }

    // public List<Vector2> GetAllLinkedItem(int index)
    // {
    //     int[] originIndex = GetInventoryIndex(index);
    //     ItemData originItem = inventoryInfo.inventoryItemData[originIndex[0], originIndex[1]];
    //     int maxSize = originItem.itemSize.x < originItem.itemSize.y ? (int)originItem.itemSize.y : (int)originItem.itemSize.x;

    //     List<Vector2> returnValue = new List<Vector2>();

    //     for (int i = originIndex[0]; i < originIndex[0] + maxSize; i++)
    //     {
    //         for (int j = originIndex[1]; j < originIndex[1] + maxSize; j++)
    //         {
    //             if (inventoryInfo.inventoryItemLink[i, j] == index)
    //             {
    //                 returnValue.Add(new Vector2(i, j));
    //             }
    //         }
    //     }

    //     return returnValue;
    // }
}
