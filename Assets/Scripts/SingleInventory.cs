using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleInventory : MonoBehaviour, IInventorySystem
{
    private InventoryInfo inventoryInfo;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(64);
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
            index = FindEmptySlot();
            if (index != -1)
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
        inventoryInfo.inventoryItemData[index] = item;
        ShowInventory();
    }

    public void RemoveItem(ItemData item)
    {
        int index = FindItemIndex(item, true);

        if (index != -1)
        {
            ItemData invItem = inventoryInfo.inventoryItemData[index];

            if (invItem.itemCount >= item.itemCount)
            {
                invItem.itemCount -= item.itemCount;
            }
            else
            {
                item.itemCount = item.itemCount - invItem.itemCount;
                invItem.itemCount = 0;
                RemoveItem(item);
            }
        }
        ShowInventory();
    }

    public void RemoveItemFromSlot(int index, int count)
    {
        if (count == -1)
        {
            inventoryInfo.inventoryItemData[index] = ItemDataBase.instance.NullItem();
            return;
        }

        inventoryInfo.inventoryItemData[index].itemCount -= count;
        if (inventoryInfo.inventoryItemData[index].itemCount <= 0)
        {
            inventoryInfo.inventoryItemData[index] = ItemDataBase.instance.NullItem();
        }
        ShowInventory();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {
        ItemData item = inventoryInfo.inventoryItemData[fromIndex];
        inventoryInfo.inventoryItemData[toIndex] = item;
        inventoryInfo.inventoryItemData[fromIndex] = ItemDataBase.instance.NullItem();
    }

    public void ShowInventory()
    {
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
                    JsonTranslate.instance.TranslateText(itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    GameObject newItemObj = Instantiate(ItemDataBase.instance.itemObjPrefab, slotObj);
                    newItemObj.transform.position = slotObj.position;
                    newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                    JsonTranslate.instance.TranslateText(newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>());
                    newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                    JsonTranslate.instance.TranslateText(newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>());
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
        for (int i = 0; i < inventoryInfo.inventoryItemData.GetLength(0); i++)
        {
            if (inventoryInfo.inventoryItemData[i].itemIndex == 0)
            {
                return i;
            }
        }

        return -1;
    }
}
