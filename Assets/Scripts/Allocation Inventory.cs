using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocationInventory : MonoBehaviour, IInventorySystem
{
    private InventoryInfo inventoryInfo;
    private int sizeX;
    private int sizeY;

    void Awake()
    {
        inventoryInfo = new InventoryInfo(InventoryCategory.Allocation, 64);
        sizeX = 8; sizeY = 8;
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
                ChangeAllItemCount(index, item.itemCount);
            }
            else
            {
                int maintainCount = item.itemMaxCount - invItemCount;
                ChangeAllItemCount(index, item.itemMaxCount + maintainCount);

                item.itemCount -= maintainCount;
                GetItem(item);
            }
        }
        else
        {
            index = FindEmptySlotWithSize(item.itemSize);
            if (index != -1)
            {
                int leftCount = item.itemCount - item.itemMaxCount;

                List<int> itemList = GetOtherItemsWithSize(index, item.itemSize);
                foreach (int i in itemList)
                {
                    inventoryInfo.inventoryItemData[i] = item;
                    inventoryInfo.inventoryItemLink[i] = index;

                    if (leftCount > 0)
                        inventoryInfo.inventoryItemData[i].itemCount = item.itemMaxCount;
                }

                if (leftCount > 0)
                {
                    ItemData newItem = item;
                    newItem.itemCount = leftCount;
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
        if (CheckSizeAble(index, item.itemSize))
        {
            List<int> itemIndexs = GetOtherItemsWithSize(index, item.itemSize);
            foreach (int i in itemIndexs)
            {
                inventoryInfo.inventoryItemData[i] = item;
                inventoryInfo.inventoryItemLink[i] = index;
            }
        }

        ShowInventory();
    }

    public void RemoveItem(ItemData item)
    {
        int index = FindItemIndex(item, true);

        if (index != -1)
        {
            List<int> itemIndexs = GetOtherItemsWithSize(index, inventoryInfo.inventoryItemData[index].itemSize);
            int invItemCount = inventoryInfo.inventoryItemData[index].itemCount;

            if (invItemCount >= item.itemCount)
            {
                foreach (int i in itemIndexs)
                {
                    inventoryInfo.inventoryItemData[i].itemCount -= item.itemCount;
                }
            }
            else
            {
                item.itemCount -= invItemCount;
                foreach (int i in itemIndexs)
                {
                    inventoryInfo.inventoryItemData[i] = ItemDataBase.instance.NullItem();
                    inventoryInfo.inventoryItemLink[i] = -1;
                }
                RemoveItem(item);
            }
            
            ShowInventory();
        }
    }

    public void RemoveItemFromSlot(int index, int count)
    {
        List<int> itemIndexs = GetOtherItemsWithSize(index, inventoryInfo.inventoryItemData[index].itemSize);
        if (count == -1)
        {
            foreach (int i in itemIndexs)
            {
                inventoryInfo.inventoryItemData[i] = ItemDataBase.instance.NullItem();
                inventoryInfo.inventoryItemLink[i] = -1;
            }
            ShowInventory();
            return;
        }

        if (inventoryInfo.inventoryItemData[index].itemCount > count)
        {
            foreach (int i in itemIndexs)
            {
                inventoryInfo.inventoryItemData[i].itemCount -= count;
            }
        }
        else
        {
            foreach (int i in itemIndexs)
            {
                inventoryInfo.inventoryItemData[i] = ItemDataBase.instance.NullItem();
                inventoryInfo.inventoryItemLink[i] = -1;
            }
        }

        ShowInventory();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {

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

                    itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "";
                    itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "";

                    if (inventoryInfo.inventoryItemLink[index] == index)
                    {
                        itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                        JsonTranslate.instance.TranslateText(itemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>());
                    }

                    if (GetCountItem(index) == index)
                    {
                        itemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                    }
                }
                else
                {
                    GameObject newItemObj = Instantiate(ItemDataBase.instance.itemObjPrefab, slotObj);
                    newItemObj.transform.position = slotObj.position;

                    newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "";
                    newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "";

                    if (inventoryInfo.inventoryItemLink[index] == index)
                    {
                        newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                        JsonTranslate.instance.TranslateText(newItemObj.transform.Find("Name").GetComponent<TextMeshProUGUI>());
                    }

                    if (GetCountItem(index) == index)
                    {
                        newItemObj.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = item.itemCount.ToString();
                    }
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
            int ownerIndex = GetOwnerItem(i);

            if (ownerIndex != -1)
            {
                if (inventoryInfo.inventoryItemData[ownerIndex].itemIndex == item.itemIndex)
                {
                    if (includeMaxStack)
                    {
                        return i;
                    }
                    else
                    {
                        if (inventoryInfo.inventoryItemData[ownerIndex].itemCount != inventoryInfo.inventoryItemData[ownerIndex].itemMaxCount)
                        {
                            return i;
                        }
                    }
                }
            }
        }

        return -1;
    }

    public int FindEmptySlot()
    {
        return -1;
    }

    public int FindEmptySlotWithSize(Vector2 size)
    {
        for (int i = 0; i < inventoryInfo.inventoryItemData.Length; i++)
        {
            if (CheckSizeAble(i, size))
            {
                return i;
            }
        }

        return -1;
    }

    private void ChangeAllItemCount(int index, int count)
    {
        List<int> items = GetOtherItems(index);

        foreach (int i in items)
        {
            inventoryInfo.inventoryItemData[i].itemCount += count;
        }
    }

    public int GetOwnerItem(int index)
    {
        return inventoryInfo.inventoryItemLink[index];
    }

    public List<int> GetOtherItems(int index)
    {
        List<int> returnValue = new List<int>();
        Vector2 size = inventoryInfo.inventoryItemData[index].itemSize;

        int startX = index % sizeX;
        int startY = index / sizeY;

        for (int y = 0; y < (int)size.y; y++)
        {
            for (int x = 0; x < (int)size.x; x++)
            {
                int checkIndex = (startY + y) * sizeY + (startX + x);

                if (!returnValue.Contains(checkIndex))
                    returnValue.Add(checkIndex);
            }
        }

        return returnValue;
    }

    public List<int> GetOtherItemsWithSize(int index, Vector2 size)
    {
        List<int> returnValue = new List<int>();

        int startX = index % sizeX;
        int startY = index / sizeY;

        for (int y = 0; y < (int)size.y; y++)
        {
            for (int x = 0; x < (int)size.x; x++)
            {
                int checkIndex = (startY + y) * sizeY + (startX + x);

                if (!returnValue.Contains(checkIndex))
                    returnValue.Add(checkIndex);
            }
        }

        return returnValue;
    }

    public int GetCountItem(int index)
    {
        Vector2 size = inventoryInfo.inventoryItemData[index].itemSize;
        return GetOwnerItem(index) + (sizeY * ((int)size.y - 1)) + ((int)size.x - 1);
    }

    public bool CheckSizeAble(int index, Vector2 size)
    {
        int startX = index % sizeX;
        int startY = index / sizeX;

        if (startX + (int)size.x > sizeX || startY + (int)size.y > sizeY)
            return false;

        for (int y = 0; y < (int)size.y; y++)
        {
            for (int x = 0; x < (int)size.x; x++)
            {
                int checkIndex = (startY + y) * sizeX + (startX + x);

                if (inventoryInfo.inventoryItemData[checkIndex].itemCount != 0)
                    return false;
            }
        }

        return true;
    }
}
