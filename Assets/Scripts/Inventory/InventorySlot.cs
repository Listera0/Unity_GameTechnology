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
        SetSlotIndex();

        canvas = GetComponentInParent<Canvas>();
        ownInventory = FindOwnInventory();
        dragSlot = InventoryManager.instance.dragSlot;
        isDragging = false;
    }

    public void SetSlotIndex()
    { 
        slotIndex = transform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.dragSlot && transform.childCount != 0)
        {
            itemObj = transform.GetChild(0).gameObject;

            if (ownInventory.GetInventoryCategory() == InventoryCategory.Allocation)
            {
                AllocationInventory inv = (AllocationInventory)ownInventory;
                int ownerIndex = inv.GetOwnerItem(slotIndex);

                InventoryManager.instance.dragOffset = slotIndex - ownerIndex;
                InventoryManager.instance.movingItemData = inv.GetInventoryItem(ownerIndex);

                List<int> items = inv.GetOtherItems(ownerIndex);
                foreach (int ind in items)
                {
                    GameObject invObj = inv.GetInventorySlotObj(ind).transform.GetChild(0).gameObject;
                    invObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    invObj.transform.SetParent(dragSlot.transform);
                    invObj.transform.localPosition = GetOffsetLocation(slotIndex - ind, 8);
                }
            }
            else if (ownInventory.GetInventoryCategory() == InventoryCategory.Extended)
            {
                // none work
            }
            else
            {
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

        if (ownInventory.GetInventoryCategory() == InventoryCategory.Allocation)
        {
            if (itemObj.transform.parent == dragSlot.transform)
            {
                foreach (Transform child in dragSlot.transform)
                {
                    child.SetParent(InventoryManager.instance.trashObject);
                    child.localPosition = Vector3.zero;
                }
                InventoryManager.instance.ClearTrash();
                AllocationInventory inv = (AllocationInventory)ownInventory;
                inv.ShowInventory();
            }
            else
            {
                itemObj = null;
            }
        }
        else if (ownInventory.GetInventoryCategory() == InventoryCategory.Extended)
        {
            // none work
        }
        else
        {
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
    }

    // there's no merge option
    public void OnDrop(PointerEventData eventData)
    {
        if (ownInventory.GetInventoryCategory() == InventoryCategory.Allocation)
        {
            if (dragSlot.transform.childCount != 0 && InventoryManager.instance.dragIndex != slotIndex)
            {
                AllocationInventory inv = (AllocationInventory)ownInventory;
                int offset = InventoryManager.instance.dragOffset;

                if (inv.GetInventoryItem(slotIndex).itemIndex == InventoryManager.instance.movingItemData.itemIndex)
                {
                    ownInventory.MoveItemToSlot(InventoryManager.instance.dragIndex, slotIndex);
                    return;
                }

                if (inv.CheckSizeAble(slotIndex - offset, InventoryManager.instance.movingItemData.itemSize))
                {
                    List<Transform> childs = new List<Transform>();

                    foreach (Transform child in dragSlot.transform)
                    {
                        childs.Add(child);
                    }

                    foreach (Transform child in childs)
                    {
                        child.SetParent(InventoryManager.instance.trashObject);
                        child.localPosition = Vector3.zero;
                    }

                    InventoryManager.instance.ClearTrash();
                    ownInventory.MoveItemToSlot(InventoryManager.instance.dragIndex, slotIndex - offset);
                    return;
                }
            }
        }
        else if (ownInventory.GetInventoryCategory() == InventoryCategory.Extended)
        {
            // none work
        }
        else
        {
            if (dragSlot.transform.childCount != 0 && transform.childCount == 0 && InventoryManager.instance.dragIndex != slotIndex)
            {
                itemObj = dragSlot.transform.GetChild(0).gameObject;
                itemObj.transform.SetParent(transform);
                itemObj.transform.localPosition = Vector3.zero;
                ownInventory.MoveItemToSlot(InventoryManager.instance.dragIndex, slotIndex);
                return;
            }
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

    private Vector3 GetOffsetLocation(int offset, int size)
    {
        int yOffset = offset / size;
        int xOffset = -(offset - yOffset * size);
        int slotSize = 80;

        Vector3 returnVector = new Vector3(slotSize * xOffset, slotSize * yOffset, 0);
        return returnVector;
    }
}
