using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class InventoryItem : MonoBehaviour
{
    Item item;
    [SerializeField] Image itemGraphic;
    [SerializeField] Transform dragParent;
    InventoryManager handler;
    public void SetData(InventoryManager _handler, Item newItem, Transform dragParent)
    {
        handler = _handler;
        item = newItem;
        itemGraphic.sprite = item.icon;
        itemGraphic.GetComponent<DraggableItem>().whileDragParent = dragParent;
        itemGraphic.GetComponent<DraggableItem>().handler = handler;
        itemGraphic.GetComponent<DraggableItem>().moveTarget = handler.moveTargetToSet;
        itemGraphic.GetComponent<DraggableItem>().item = newItem;
    }
}
