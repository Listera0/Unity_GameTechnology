using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;

    private Canvas canvas;
    private GameObject itemObj;
    private IInventorySystem ownInventory;

    void Awake()
    {
        slotIndex = transform.GetSiblingIndex();

        canvas = GetComponentInParent<Canvas>();
        ownInventory = FindOwnInventory();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.dragSlot && transform.childCount != 0)
        {
            itemObj = transform.GetChild(0).gameObject;
            itemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
            itemObj.transform.SetParent(InventoryManager.instance.dragSlot.transform);
            InventoryManager.instance.dragIndex = slotIndex;
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

        if (itemObj.transform.parent == InventoryManager.instance.dragSlot.transform)
        {
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            itemObj = null;
            ownInventory.RemoveItemFromSlot(slotIndex, -1);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryManager.instance.dragSlot.transform.childCount != 0 && InventoryManager.instance.dragSlot.transform.GetChild(0) != itemObj)
        {
            itemObj = InventoryManager.instance.dragSlot.transform.GetChild(0).gameObject;
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
            ownInventory.MoveItemToSlot(InventoryManager.instance.dragIndex, slotIndex);
        }
    }

    private IInventorySystem FindOwnInventory()
    {
        Transform findobj = transform.parent;

        for (int i = 0; i < 3; i++)
        {
            if (findobj.GetComponent<IInventorySystem>() != null)
            {
                return findobj.GetComponent<IInventorySystem>();
            }
            findobj = findobj.parent;
        }

        return null;
    }
}
