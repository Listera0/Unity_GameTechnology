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
    public Vector2 itemSize;

    public ItemData(int index, ItemCategory category, string name, int count, int maxCount, Vector2 size)
    {
        itemIndex = index;
        itemCategory = category;
        itemName = name;
        itemCount = count;
        itemMaxCount = maxCount;
        itemSize = size;
    }
}

public class ItemDataBase : Singleton<ItemDataBase>, IInitializeInter
{
    public List<ItemData> itemDatabases;
    public GameObject itemObjPrefab;

    public void Initialize()
    {
        itemDatabases = new List<ItemData>();
        if (!itemObjPrefab) Debug.LogWarning("itemObjPrefab is Null");
        
        SetItemDataBase();
    }

    public void SetItemDataBase()
    {
        AddItemDataBase(ItemCategory.Others, "None", 0, new Vector2(0, 0));
        AddItemDataBase(ItemCategory.Consumeable, "Water", 10, new Vector2(2, 1));
        AddItemDataBase(ItemCategory.Consumeable, "Food", 10, new Vector2(2, 1));
        AddItemDataBase(ItemCategory.Others, "Coin", 10, new Vector2(1, 1));
    }

    public void AddItemDataBase(ItemCategory category, string name, int maxCount, Vector2 size)
    {
        itemDatabases.Add(new ItemData(itemDatabases.Count, category, name, 0, maxCount, size));
    }

    public ItemData GetItemData(int index)
    {
        return itemDatabases[index];
    }

    public ItemData NullItem()
    {
        return itemDatabases[0];
    }
}
