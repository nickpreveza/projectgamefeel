using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class InventoryItem : MonoBehaviour
{
    Item item;
    [SerializeField] Image itemGraphic;
    [SerializeField] Transform dragParent;

    public void SetData(Item newItem, Transform dragParent)
    {
        item = newItem;
        itemGraphic.sprite = item.icon;
        itemGraphic.GetComponent<DraggableItem>().whileDragParent = dragParent;
    }
}
