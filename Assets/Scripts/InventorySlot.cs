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
    private ItemData itemData;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventorySystem.instance.dragSlot && transform.childCount != 0)
        {
            itemObj = transform.GetChild(0).gameObject;
            itemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
            itemObj.transform.SetParent(InventorySystem.instance.dragSlot.transform);
            InventorySystem.instance.movingItemData = itemData;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!itemObj) return;

        itemObj.transform.position = eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (itemObj.transform.parent == InventorySystem.instance.dragSlot.transform)
        {
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySystem.instance.dragSlot.transform.childCount != 0)
        {
            itemObj = InventorySystem.instance.dragSlot.transform.GetChild(0).gameObject;
            itemObj.transform.SetParent(transform);
            itemObj.transform.localPosition = Vector3.zero;
            itemData = InventorySystem.instance.movingItemData;
        }
    }
}
