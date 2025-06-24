using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum ItemCategory
{
    Equipment = 0,
    Consumeable,
    Others
}

[SerializeField]
public struct ItemData
{
    public int itemIndex;
    public ItemCategory itemCategory;
    public string itemName;
    public int itemCount;
    public int itemMaxCount;

    public ItemData(int index, ItemCategory category, string name, int count, int maxCount)
    {
        itemIndex = index;
        itemCategory = category;
        itemName = name;
        itemCount = count;
        itemMaxCount = maxCount;
    }

    public ItemData(ItemData copyed)
    {
        itemIndex = copyed.itemIndex;
        itemCategory = copyed.itemCategory;
        itemName = copyed.itemName;
        itemCount = copyed.itemCount;
        itemMaxCount = copyed.itemMaxCount;
    }
}

public class ItemDataBase : Singleton<ItemDataBase>, IInitializeInter
{
    public List<ItemData> itemDatabases;

    public void Initialize()
    {
        itemDatabases = new List<ItemData>();
        SetItemDataBase();
    }

    public void SetItemDataBase()
    {
        AddItemDataBase(ItemCategory.Others, "None", 0);
        AddItemDataBase(ItemCategory.Consumeable, "Water", 100);
        AddItemDataBase(ItemCategory.Consumeable, "Food", 100);
        AddItemDataBase(ItemCategory.Others, "Coin", 1000);
    }

    public void AddItemDataBase(ItemCategory category, string name, int maxCount)
    {
        itemDatabases.Add(new ItemData(itemDatabases.Count, category, name, 0, maxCount));
    }

    public ItemData GetItemData(int index)
    {
        return new ItemData(itemDatabases[index]);
    }

    public ItemData NullItem()
    {
        return new ItemData(itemDatabases[0]);
    }
}
