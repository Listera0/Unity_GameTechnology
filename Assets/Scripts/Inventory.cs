using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInfo
{
    public ItemData[,] inventoryItemData;
    public int[,] inventoryItemCount;

    public InventoryInfo(int sizeX, int sizeY)
    {
        inventoryItemData = new ItemData[sizeX, sizeY];
        inventoryItemCount = new int[sizeX, sizeY];
    }
}

public enum InventoryCategory
{
    Single = 0,
    Classification
}

public class Inventory : MonoBehaviour
{
    public InventoryCategory inventoryCategory;

    private InventoryInfo singleInventory;

    void Awake()
    {
        singleInventory = new InventoryInfo(8, 8);
    }

    public void GetItemToInventory(ItemData itmeData, int slotIndex)
    {
        switch (inventoryCategory)
        {
            case InventoryCategory.Single:
                SingleInventoryGetItem(itmeData, -1);
                break;
        }
    }

    public void RemoveItemFromInventory(ItemData itemData)
    {

    }

    public void RemoveItemFromInventory(int itemSlot)
    {

    }

    private void SingleInventoryGetItem(ItemData itemData, int itemIndex)
    {

    }

    private void SingleInventoryRemoveItem(ItemData itemData, int itemSlot)
    {
        
    }
}
