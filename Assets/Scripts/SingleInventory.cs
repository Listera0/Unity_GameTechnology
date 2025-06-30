using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleInventory : MonoBehaviour, IInventorySystem
{
    private InventoryInfo inventoryInfo;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(8, 8);
    }

    public void GetItem(ItemData item)
    {
        int[] index = FindItemIndex(item, false);
        if (index[0] != -1)
        {
            int invItemCount = inventoryInfo.inventoryItemData[index[0], index[1]].itemCount;

            if (invItemCount + item.itemCount <= item.itemMaxCount)
            {
                inventoryInfo.inventoryItemData[index[0], index[1]].itemCount += item.itemCount;
            }
            else
            {
                item.itemCount -= item.itemMaxCount - invItemCount;
                inventoryInfo.inventoryItemData[index[0], index[1]].itemCount = item.itemMaxCount;
                GetItem(item);
            }
        }
        else
        {
            index = FindEmptySlot();
            if (index[0] != -1)
            {
                ItemData invItem = inventoryInfo.inventoryItemData[index[0], index[1]] = item;

                if (invItem.itemCount > invItem.itemMaxCount)
                {
                    int leftCount = invItem.itemCount - invItem.itemMaxCount;
                    ItemData newItem = new ItemData(invItem);
                    newItem.itemCount = leftCount;
                    inventoryInfo.inventoryItemData[index[0], index[1]].itemCount = invItem.itemMaxCount;
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
        int[] Iindex = GetInventoryIndex(index);

        inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = item;
        ShowInventory();
    }

    public void RemoveItem(ItemData item)
    {
        int[] index = FindItemIndex(item, true);

        if (index[0] != -1)
        {
            ItemData invItem = inventoryInfo.inventoryItemData[index[0], index[1]];

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
        int[] Iindex = GetInventoryIndex(index);

        if (count == -1)
        {
            inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
            return;
        }

        inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]].itemCount -= count;
        if (inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]].itemCount <= 0)
        {
            inventoryInfo.inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
        }
        ShowInventory();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {
        int[] FIndex = GetInventoryIndex(fromIndex);
        int[] TIndex = GetInventoryIndex(toIndex);

        ItemData item = inventoryInfo.inventoryItemData[FIndex[0], FIndex[1]];
        inventoryInfo.inventoryItemData[TIndex[0], TIndex[1]] = item;
        inventoryInfo.inventoryItemData[FIndex[0], FIndex[1]] = ItemDataBase.instance.NullItem();
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
                    itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                }
                else
                {
                    GameObject newItemObj = Instantiate(ItemDataBase.instance.itemObjPrefab, slotObj);
                    newItemObj.transform.position = slotObj.position;
                    newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
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

    public int[] FindItemIndex(ItemData item, bool includeMaxStack)
    {
        int[] returnValue = new int[2] { -1, -1 };

        for (int i = 0; i < inventoryInfo.inventoryItemData.GetLength(0); i++)
        {
            for (int j = 0; j < inventoryInfo.inventoryItemData.GetLength(1); j++)
            {
                if (inventoryInfo.inventoryItemData[i, j].itemIndex == item.itemIndex)
                {
                    if (includeMaxStack)
                    {
                        return new int[2] { i, j };
                    }
                    else
                    {
                        if (inventoryInfo.inventoryItemData[i, j].itemCount != inventoryInfo.inventoryItemData[i, j].itemMaxCount)
                        {
                            return new int[2] { i, j };
                        }
                    }
                }
            }
        }

        return returnValue;
    }

    public int[] FindEmptySlot()
    {
        int[] returnValue = new int[2] { -1, -1 };

        for (int i = 0; i < inventoryInfo.inventoryItemData.GetLength(0); i++)
        {
            for (int j = 0; j < inventoryInfo.inventoryItemData.GetLength(1); j++)
            {
                if (inventoryInfo.inventoryItemData[i, j].itemIndex == 0)
                {
                    return new int[2] { i, j };
                }
            }
        }

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
