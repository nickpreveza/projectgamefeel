using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class InventoryItem : MonoBehaviour
{
    Item item;
    [SerializeField] Image itemGraphic;
    [SerializeField] Transform dragParent;

    [SerializeField] TextMeshProUGUI value;
    [SerializeField] StatUI str;
    [SerializeField] StatUI con;
    [SerializeField] StatUI dex;
    InventoryManager handler;
    public void SetData(InventoryManager _handler, Item newItem, Transform dragParent, bool armymode)
    {
        handler = _handler;
        item = newItem;
        itemGraphic.sprite = item.icon;

        DraggableItem draggableItem = itemGraphic.GetComponent<DraggableItem>();
        draggableItem.whileDragParent = dragParent;
        draggableItem.handler = handler;
        draggableItem.SetUpOriginAndTarget(handler.parentType, handler.sendTarget);
        draggableItem.item = newItem;

        if (armymode)
        {
            draggableItem.SetUpUnit(item, dragParent, handler.parentType, handler.sendTarget);
        }
        if (value != null)
        {
            value.text = item.buyValue.ToString();
            str.UpdateVisuals(item.strRequirment);
            con.UpdateVisuals(item.conRequirment);
            dex.UpdateVisuals(item.dexRequirment);

        }
    }
}
