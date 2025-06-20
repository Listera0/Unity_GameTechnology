using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas canvas;
    private GameObject itemObj;
    private Inventory ownInventory;
    private int slotIndex;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        ownInventory = FindOwnInventory();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventorySystem.instance.dragSlot && transform.childCount != 0)
        {
            itemObj = transform.GetChild(0).gameObject;
            itemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
            itemObj.transform.SetParent(InventorySystem.instance.dragSlot.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!itemObj) return;

        // itemObj.transform.position = eventData.delta / canvas.scaleFactor;
        itemObj.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!itemObj) return;
        // itemObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (itemObj.transform.parent == InventorySystem.instance.dragSlot.transform)
        {
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            itemObj = null;
            ownInventory.RemoveItemFromInventory(slotIndex);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySystem.instance.dragSlot.transform.childCount != 0 && InventorySystem.instance.dragSlot.transform.GetChild(0) != itemObj)
        {
            itemObj = InventorySystem.instance.dragSlot.transform.GetChild(0).gameObject;
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
            ownInventory.GetItemToInventory(ItemDataBase.instance.itemDatabases[0], slotIndex);
        }
    }

    private Inventory FindOwnInventory()
    {
        Transform findobj = transform.parent;

        for (int i = 0; i < 3; i++)
        {
            if (findobj.GetComponent<Inventory>() != null)
            {
                return findobj.GetComponent<Inventory>();
            }
            findobj = transform.parent;
        }

        return null;
    }
}
