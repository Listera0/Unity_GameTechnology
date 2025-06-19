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
    public string itemName;
    public ItemCategory itemCategory;

    public ItemData(int index, string name, ItemCategory category)
    {
        itemIndex = index;
        itemName = name;
        itemCategory = category;
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
        AddItemDataBase("Water", ItemCategory.Consumeable);
        AddItemDataBase("Food", ItemCategory.Consumeable);
        AddItemDataBase("Coin", ItemCategory.Others);
    }

    public void AddItemDataBase(string name, ItemCategory category)
    {
        itemDatabases.Add(new ItemData(itemDatabases.Count, name, category));
    }
}
