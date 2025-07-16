using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtendedInventory : MonoBehaviour, IInventorySystem
{
    public GameObject invSlotPrefab;

    private InventoryInfo inventoryInfo;
    private int maxInventorySlotCount = 64;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(InventoryCategory.Extended, 0);
    }

    public InventoryCategory GetInventoryCategory()
    {
        return inventoryInfo.inventoryCategory;
    }

    public void GetItem(ItemData item)
    {
        int index = FindItemIndex(item, false);
        if (index != -1)
        {
            int invItemCount = inventoryInfo.inventoryItemData[index].itemCount;

            if (invItemCount + item.itemCount <= item.itemMaxCount)
            {
                inventoryInfo.inventoryItemData[index].itemCount += item.itemCount;
            }
            else
            {
                item.itemCount -= item.itemMaxCount - invItemCount;
                inventoryInfo.inventoryItemData[index].itemCount = item.itemMaxCount;
                GetItem(item);
            }
        }
        else
        {
            ExtendSlot(true);
            index = inventoryInfo.inventoryItemData.Length - 1;
            if (index < maxInventorySlotCount)
            {
                ItemData invItem = inventoryInfo.inventoryItemData[index] = item;

                if (invItem.itemCount > invItem.itemMaxCount)
                {
                    int leftCount = invItem.itemCount - invItem.itemMaxCount;
                    ItemData newItem = invItem;
                    newItem.itemCount = leftCount;
                    inventoryInfo.inventoryItemData[index].itemCount = invItem.itemMaxCount;
                    GetItem(newItem);
                }
            }
            else
            {
                Debug.LogWarning("No more empty inventory slot");
            }
        }
        ShowInventory();
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
        if (inventoryInfo.inventoryItemData.Length == 0) return;

        int index = 0;
        foreach (ItemData item in inventoryInfo.inventoryItemData)
        {
            Transform slotObj = transform.GetChild(0).GetChild(index);

            if (item.itemCount != 0)
            {
                if (slotObj.childCount != 0)
                {
                    GameObject itemObj = slotObj.GetChild(0).gameObject;
                    itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                    JsonTranslate.instance.TranslateText(itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>());
                    itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                }
                else
                {
                    GameObject newItemObj = Instantiate(ItemDataBase.instance.itemObjPrefab, slotObj);
                    newItemObj.transform.position = slotObj.position;
                    newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                    JsonTranslate.instance.TranslateText(newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>());
                    newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                }
            }
            else
            {
                if (slotObj.childCount != 0)
                {
                    Destroy(slotObj.GetChild(0).gameObject);
                }
            }

            index++;
        }
    }

    public int FindItemIndex(ItemData item, bool includeMaxStack)
    {
        for (int i = 0; i < inventoryInfo.inventoryItemData.Length; i++)
        {
            if (inventoryInfo.inventoryItemData[i].itemIndex == item.itemIndex)
            {
                if (includeMaxStack)
                {
                    return i;
                }
                else
                {
                    if (inventoryInfo.inventoryItemData[i].itemCount != inventoryInfo.inventoryItemData[i].itemMaxCount)
                    {
                        return i;
                    }
                }
            }
        }

        return -1;
    }

    public int FindEmptySlot()
    {
        for (int i = 0; i < inventoryInfo.inventoryItemData.Length; i++)
        {
            if (inventoryInfo.inventoryItemData[i].itemIndex == 0)
            {
                return i;
            }
        }

        return -1;
    }

    public void ExtendSlot(bool extended)
    {
        int prevSize = inventoryInfo.inventoryItemData.Length;
        int newSize = extended ? prevSize + 1 : prevSize - 1;
        InventoryInfo newInventory = new InventoryInfo(InventoryCategory.Extended, newSize);

        for (int i = 0; i < Math.Min(prevSize, newSize); i++)
        {
            newInventory.inventoryItemData[i] = inventoryInfo.inventoryItemData[i];
        }

        inventoryInfo = newInventory;

        Transform inventoryObj = transform.Find("Inventory Obj");
        int slotCount = inventoryObj.childCount;
        
        if (slotCount < newSize)
        {
            for (int i = slotCount; i < newSize; i++)
            {
                Instantiate(invSlotPrefab, inventoryObj);
            }
        }
        else if (slotCount > newSize)
        {
            for (int i = slotCount - 1; i >= newSize; i--)
            {
                Destroy(inventoryObj.GetChild(i).gameObject);
            }
        }
    }
}
