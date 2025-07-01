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
    private int selectCategoryIndex;

    void Awake()
    {
        inventoryInfo = new InventoryInfo[Enum.GetValues(typeof(ItemCategory)).Length];
        for (int i = 0; i < inventoryInfo.Length; i++)
        {
            inventoryInfo[i] = new InventoryInfo(8, 8);
        }
        
        SetCategoryArea();
    }

    public void AddItem(ItemData item, int index)
    {
        throw new System.NotImplementedException();
    }

    public int[] FindEmptySlot()
    {
        throw new System.NotImplementedException();
    }

    public int[] FindItemIndex(ItemData item, bool includeMaxStack)
    {
        throw new System.NotImplementedException();
    }

    public int[] GetInventoryIndex(int index)
    {
        throw new System.NotImplementedException();
    }

    public void GetItem(ItemData item)
    {
        throw new System.NotImplementedException();
    }

    public void MoveItemToSlot(int fromIndex, int toIndex)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItem(ItemData item)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItemFromSlot(int index, int count)
    {
        throw new System.NotImplementedException();
    }

    public void ShowInventory()
    {
        throw new System.NotImplementedException();
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
            obj.GetComponent<Button>().onClick.AddListener(() => SelectIndex(sortIndex));
            radioSelectButton.selectObject[index] = obj;
            index++;
        }

        radioSelectButton.RadioSelectButtonInit();
        radioSelectButton.UnVisibleSelect(0);
    }

    public void SelectIndex(int index) 
    {
        selectCategoryIndex = index;
    }
}
