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
    private GameObject dragSlot;
    private bool isDragging;

    void Awake()
    {
        slotIndex = transform.GetSiblingIndex();

        canvas = GetComponentInParent<Canvas>();
        ownInventory = FindOwnInventory();
        dragSlot = InventoryManager.instance.dragSlot;
        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.dragSlot && transform.childCount != 0)
        {
            if (ownInventory.GetInventoryCategory() == InventoryCategory.Allocation)
            {
                AllocationInventory inv = (AllocationInventory)ownInventory;
                List<int> items = inv.GetOtherItems(slotIndex);
            }
            else if (ownInventory.GetInventoryCategory() == InventoryCategory.Extended)
            {
                // none work
            }
            else
            {
                itemObj = transform.GetChild(0).gameObject;
                itemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
                itemObj.transform.SetParent(dragSlot.transform);
                itemObj.transform.localPosition = Vector3.zero;
            }

            InventoryManager.instance.dragIndex = slotIndex;
            dragSlot.transform.position = eventData.position;
            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        dragSlot.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        if (itemObj.transform.parent == dragSlot.transform)
        {
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            itemObj = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (dragSlot.transform.childCount != 0 && dragSlot.transform.GetChild(0) != itemObj)
        {
            itemObj = dragSlot.transform.GetChild(0).gameObject;
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
