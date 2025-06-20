using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMaker : MonoBehaviour, IInitializeInter
{
    public GameObject itemButtonListPanel;
    public GameObject itemButtonListArea;
    public GameObject indexShowButton;
    public GameObject itemButtonPrefab;
    public TextMeshProUGUI itemInformation;
    public RadioSelectButton InventoryPanelArea;

    private List<GameObject> currentItemButtons;
    private int selectItemIndex;

    public void Initialize()
    {
        if (!itemButtonListPanel) Debug.LogWarning("itemButtonList is Null");
        if (!itemButtonListArea) Debug.LogWarning("itemButtonListArea is Null");
        if (!indexShowButton) Debug.LogWarning("indexShowButton is Null");
        else indexShowButton.GetComponent<Button>().onClick.AddListener(ToggleItemIndexButtonList);

        if (!itemButtonPrefab) Debug.LogWarning("itemButtonPrefab is Null");
        if (!itemInformation) Debug.LogWarning("itemInformation is Null");

        currentItemButtons = new List<GameObject>();
    }

    public void ToggleItemIndexButtonList()
    {
        if (!itemButtonListPanel) return;

        if (itemButtonListPanel.activeSelf)
        {
            itemButtonListPanel.SetActive(false);
        }
        else
        {
            itemButtonListPanel.SetActive(true);
            SetIndexButtonList();
        }
    }

    public void SetCurrentItemIndex(int index)
    {
        indexShowButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = index.ToString();
        selectItemIndex = index;
        SetCurrentItemInformation(index);
        ToggleItemIndexButtonList();
    }

    public void SetCurrentItemInformation(int index)
    {
        ItemData itemdata = ItemDataBase.instance.itemDatabases[index];

        string information = "";
        information += "Index : " + itemdata.itemIndex.ToString();
        information += "\nName : " + itemdata.itemName;
        information += "\nCategory : " + itemdata.itemCategory;

        itemInformation.text = information;
    }

    public void MakeItemToInventory()
    {
        Inventory targetInv = InventoryPanelArea.GetSelectGameObject().GetComponent<Inventory>();
        
    }

    private void SetIndexButtonList()
    {
        if (!itemButtonPrefab) return;

        int dataBaseCount = ItemDataBase.instance.itemDatabases.Count;
        int itemListCount = itemButtonListArea.transform.childCount;

        if (dataBaseCount < itemListCount)
        {
            for (int i = dataBaseCount; i < itemListCount; i++)
            {
                currentItemButtons.RemoveAt(currentItemButtons.Count - 1);
            }
        }
        else if (dataBaseCount > itemListCount)
        {
            for (int i = itemListCount; i < dataBaseCount; i++)
            {
                currentItemButtons.Add(Instantiate(itemButtonPrefab, itemButtonListArea.transform));
            }
        }

        int index = 0;
        foreach (GameObject obj in currentItemButtons)
        {
            int itemIndex = ItemDataBase.instance.itemDatabases[index].itemIndex;
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemIndex.ToString();
            obj.GetComponent<Button>().onClick.RemoveAllListeners();
            obj.GetComponent<Button>().onClick.AddListener(() => SetCurrentItemIndex(itemIndex));
            index++;
        }
    }
}
