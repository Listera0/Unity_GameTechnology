using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInventory : MonoBehaviour, IInventorySystem
{
    private InventoryInfo inventoryInfo;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(8, 8);
    }

    // required
    public void GetItem(ItemData item)
    {
        
    }

    public void AddItem(ItemData item, int index)
    {
        int[] Iindex = GetInventoryIndex(index);

        inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = item;
    }

    // required
    public void RemoveItem(ItemData item)
    {
        
    }

    public void RemoveItemFromSlot(int index, int count)
    {
        int[] Iindex = GetInventoryIndex(index);

        if (count == -1)
        {
            inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
            return;
        }

        inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]].currentItemCount -= count;
        if (inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]].currentItemCount <= 0)
        {
            inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
        }
    }

    // required
    public int[] FindItemIndex(ItemData item)
    {
        int[] returnValue = new int[2] { -1, -1 };

        return returnValue;
    }

    // required
    public int[] FindEmptySlot(ItemData item)
    {
        int[] returnValue = new int[2] { -1, -1 };

        return returnValue;
    }

    public int[] GetInventoryIndex(int index)
    {
        int[] returnValue = new int[2] { -1, -1 };

        returnValue[0] = index / inventoryInfo.inventoryItemData.GetLength(0);
        returnValue[1] = index % inventoryInfo.inventoryItemData.GetLength(1);

        return returnValue;
    }
}
