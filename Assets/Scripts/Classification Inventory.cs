using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClassificationInventory : MonoBehaviour, IInventorySystem
{
    public GameObject categoryArea;
    public GameObject categoryButtonPrefab;

    private InventoryInfo[] inventoryInfo;
    private int currentCategoryIndex;
    private int itemCategoryIndex;

    void Awake()
    {
        inventoryInfo = new InventoryInfo[Enum.GetValues(typeof(ItemCategory)).Length];
        for (int i = 0; i < inventoryInfo.Length; i++)
        {
            inventoryInfo[i] = new InventoryInfo(8, 8);
        }

        SetCategoryArea();
    }

    public void GetItem(ItemData item)
    {
        itemCategoryIndex = (int)item.itemCategory;

        int[] index = FindItemIndex(item, false);
        if (index[0] != -1)
        {
            int invItemCount = inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]].itemCount;

            if (invItemCount + item.itemCount <= item.itemMaxCount)
            {
                inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]].itemCount += item.itemCount;
            }
            else
            {
                item.itemCount -= item.itemMaxCount - invItemCount;
                inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]].itemCount = item.itemMaxCount;
                GetItem(item);
            }
        }
        else
        {
            index = FindEmptySlot();
            if (index[0] != -1)
            {
                ItemData invItem = inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]] = item;

                if (invItem.itemCount > invItem.itemMaxCount)
                {
                    int leftCount = invItem.itemCount - invItem.itemMaxCount;
                    ItemData newItem = new ItemData(invItem);
                    newItem.itemCount = leftCount;
                    inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]].itemCount = invItem.itemMaxCount;
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

        inventoryInfo[itemCategoryIndex].inventoryItemData[Iindex[0], Iindex[1]] = item;
        ShowInventory();
    }

    public void RemoveItem(ItemData item)
    {
        int[] index = FindItemIndex(item, true);

        if (index[0] != -1)
        {
            ItemData invItem = inventoryInfo[itemCategoryIndex].inventoryItemData[index[0], index[1]];

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
            inventoryInfo[itemCategoryIndex].inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
            return;
        }

        inventoryInfo[itemCategoryIndex].inventoryItemData[Iindex[0], Iindex[1]].itemCount -= count;
        if (inventoryInfo[itemCategoryIndex].inventoryItemData[Iindex[0], Iindex[1]].itemCount <= 0)
        {
            inventoryInfo[itemCategoryIndex].inventoryItemData[Iindex[0], Iindex[1]] = ItemDataBase.instance.NullItem();
        }
        ShowInventory();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {
        int[] FIndex = GetInventoryIndex(fromIndex);
        int[] TIndex = GetInventoryIndex(toIndex);

        ItemData item = inventoryInfo[itemCategoryIndex].inventoryItemData[FIndex[0], FIndex[1]];
        inventoryInfo[itemCategoryIndex].inventoryItemData[TIndex[0], TIndex[1]] = item;
        inventoryInfo[itemCategoryIndex].inventoryItemData[FIndex[0], FIndex[1]] = ItemDataBase.instance.NullItem();
    }

    public void ShowInventory()
    {
        int index = 0;
        foreach (ItemData item in inventoryInfo[currentCategoryIndex].inventoryItemData)
        {
            Transform slotObj = transform.GetChild(0).GetChild(0).GetChild(index);

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

    public int[] FindItemIndex(ItemData item, bool includeMaxStack)
    {
        int[] returnValue = new int[2] { -1, -1 };

        for (int i = 0; i < inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(0); i++)
        {
            for (int j = 0; j < inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(1); j++)
            {
                if (inventoryInfo[itemCategoryIndex].inventoryItemData[i, j].itemIndex == item.itemIndex)
                {
                    if (includeMaxStack)
                    {
                        return new int[2] { i, j };
                    }
                    else
                    {
                        if (inventoryInfo[itemCategoryIndex].inventoryItemData[i, j].itemCount != inventoryInfo[itemCategoryIndex].inventoryItemData[i, j].itemMaxCount)
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

        for (int i = 0; i < inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(0); i++)
        {
            for (int j = 0; j < inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(1); j++)
            {
                if (inventoryInfo[itemCategoryIndex].inventoryItemData[i, j].itemIndex == 0)
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

        returnValue[0] = index / inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(0);
        returnValue[1] = index % inventoryInfo[itemCategoryIndex].inventoryItemData.GetLength(1);

        return returnValue;
    }

    public void SetCategoryArea()
    {
        ItemCategory[] categories = (ItemCategory[])Enum.GetValues(typeof(ItemCategory));
        RadioSelectButton radioSelectButton = categoryArea.GetComponent<RadioSelectButton>();
        radioSelectButton.selectObject = new GameObject[categories.Length];

        int index = 0;
        foreach (ItemCategory category in categories)
        {
            int sortIndex = index;
            GameObject obj = Instantiate(categoryButtonPrefab, categoryArea.transform);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = category.ToString();
            JsonTranslate.instance.TranslateText(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            obj.GetComponent<Button>().onClick.AddListener(() => SelectIndex(sortIndex));
            radioSelectButton.selectObject[index] = obj;
            index++;
        }

        radioSelectButton.RadioSelectButtonInit();
        radioSelectButton.UnVisibleSelect(0);
    }

    public void SelectIndex(int index)
    {
        currentCategoryIndex = index;
        ShowInventory();
    }
}
