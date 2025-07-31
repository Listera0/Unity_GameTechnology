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
            inventoryInfo[i] = new InventoryInfo(InventoryCategory.Classification, 64);
        }

        SetCategoryArea();
    }

    public InventoryCategory GetInventoryCategory()
    {
        return inventoryInfo[0].inventoryCategory;
    }

    public void GetItem(ItemData item)
    {
        itemCategoryIndex = (int)item.itemCategory;
        int index = FindItemIndex(item, false);

        if (index != -1)
        {
            int invItemCount = inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount;

            if (invItemCount + item.itemCount <= item.itemMaxCount)
            {
                inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount += item.itemCount;
            }
            else
            {
                item.itemCount -= item.itemMaxCount - invItemCount;
                inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount = item.itemMaxCount;
                GetItem(item);
            }
        }
        else
        {
            index = FindEmptySlot();
            if (index != -1)
            {
                ItemData invItem = inventoryInfo[itemCategoryIndex].inventoryItemData[index] = item;

                if (invItem.itemCount > invItem.itemMaxCount)
                {
                    int leftCount = invItem.itemCount - invItem.itemMaxCount;
                    ItemData newItem = invItem;
                    newItem.itemCount = leftCount;
                    inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount = invItem.itemMaxCount;
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
        inventoryInfo[itemCategoryIndex].inventoryItemData[index] = item;
        ShowInventory();
    }

    public void RemoveItem(ItemData item)
    {
        int index = FindItemIndex(item, true);

        if (index != -1)
        {
            int invItemCount = inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount;

            if (invItemCount >= item.itemCount)
            {
                inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount -= item.itemCount;
            }
            else
            {
                item.itemCount -= invItemCount;
                inventoryInfo[itemCategoryIndex].inventoryItemData[index] = ItemDataBase.instance.NullItem();
                RemoveItem(item);
            }
            
            ShowInventory();
        }
    }

    public void RemoveItemFromSlot(int index, int count)
    {
        if (count == -1)
        {
            inventoryInfo[itemCategoryIndex].inventoryItemData[index] = ItemDataBase.instance.NullItem();
            return;
        }

        inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount -= count;
        if (inventoryInfo[itemCategoryIndex].inventoryItemData[index].itemCount <= 0)
        {
            inventoryInfo[itemCategoryIndex].inventoryItemData[index] = ItemDataBase.instance.NullItem();
        }
        ShowInventory();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {
        ItemData item = inventoryInfo[itemCategoryIndex].inventoryItemData[fromIndex];
        inventoryInfo[itemCategoryIndex].inventoryItemData[toIndex] = item;
        inventoryInfo[itemCategoryIndex].inventoryItemData[fromIndex] = ItemDataBase.instance.NullItem();
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
                    itemObj.transform.Find("Name").GetComponent<DynamicTranslate>().InitAndChange(item.itemName);
                    itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                }
                else
                {
                    GameObject newItemObj = ObjectPoolManager.instance.GetObjectFromPool("Item");
                    InventoryManager.instance.SetItemSizeToSlot(newItemObj, slotObj);
                    newItemObj.transform.Find("Name").GetComponent<DynamicTranslate>().InitAndChange(item.itemName);
                    newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                }
            }
            else
            {
                if (slotObj.childCount != 0)
                {
                    ObjectPoolManager.instance.ReturnObjectToPool(slotObj.GetChild(0).gameObject);
                }
            }

            index++;
        }
    }

    public int FindItemIndex(ItemData item, bool includeMaxStack)
    {
        for (int i = 0; i < inventoryInfo[itemCategoryIndex].inventoryItemData.Length; i++)
        {
            if (inventoryInfo[itemCategoryIndex].inventoryItemData[i].itemIndex == item.itemIndex)
            {
                if (includeMaxStack)
                {
                    return i;
                }
                else
                {
                    if (inventoryInfo[itemCategoryIndex].inventoryItemData[i].itemCount != inventoryInfo[itemCategoryIndex].inventoryItemData[i].itemMaxCount)
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
        for (int i = 0; i < inventoryInfo[itemCategoryIndex].inventoryItemData.Length; i++)
        {
            if (inventoryInfo[itemCategoryIndex].inventoryItemData[i].itemIndex == 0)
            {
                return i;
            }
        }

        return -1;
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
            obj.transform.GetChild(0).GetComponent<DynamicTranslate>().InitAndChange(category.ToString());
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
