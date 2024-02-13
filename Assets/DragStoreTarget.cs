using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragStoreTarget : MonoBehaviour, IDropHandler
{
    bool animating;
    public ItemMoveTarget returnMoveTarget;
    public ItemMoveTarget localMoveTarget;
    [SerializeField] Transform dragItemsParent;
    [SerializeField] int maxItemsInContainer = 10;
    public bool hasItemsToReturn;

    [SerializeField] InventoryManager storeItems;
    [SerializeField] InventoryManager userItems;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] GameObject valueIcon;
    public StoreManager handler;

    public void AddItemsToUserInventory()
    {
        hasItemsToReturn = false;
        foreach (Transform child in dragItemsParent)
        {
            FeudGameManager.Instance.Player().ownedItems.Add(child.GetComponent<DraggableItem>().item);
            Destroy(child.gameObject);
        }
    }

    public void RemoveItemsFromUserInventory()
    {
        hasItemsToReturn = false;
        foreach (Transform child in dragItemsParent)
        {
            if (FeudGameManager.Instance.Player().ownedItems.Contains(child.GetComponent<DraggableItem>().item))
            {
                FeudGameManager.Instance.Player().ownedItems.Remove(child.GetComponent<DraggableItem>().item);
            }
            Destroy(child.gameObject);
        }
    }

    public void ClearBox()
    {
      
        if (hasItemsToReturn)
        {
            foreach (Transform child in dragItemsParent)
            {
                child.GetComponent<DraggableItem>().handler.AddItem(child.GetComponent<DraggableItem>().item, false, false);
                Destroy(child.gameObject);
            }

            hasItemsToReturn = false;
        }
        else
        {
            foreach (Transform child in dragItemsParent)
            {
                Destroy(child.gameObject);
            }
        }

        valueIcon.SetActive(false);
    }

    public int SellValue
    {
        get
        {
            int valueFound = 0;
            foreach (Transform child in dragItemsParent)
            {
                valueFound += child.GetComponent<DraggableItem>().item.sellValue;
            }

            return valueFound;
        }
    }

    public int BuyValue
    {
        get
        {
            int valueFound = 0;
            foreach (Transform child in dragItemsParent)
            {
                valueFound += child.GetComponent<DraggableItem>().item.buyValue;
            }

            return valueFound;
        }
    }

    public void UpdateValueIcons()
    {
        if (localMoveTarget == ItemMoveTarget.SELLBOX)
        {
            int sellValue = SellValue;
            if (sellValue > 0)
            {
                valueIcon.SetActive(true);
                valueText.text = sellValue.ToString();
            }
            else
            {
                valueIcon.SetActive(false);
            }
        }
        else if (localMoveTarget == ItemMoveTarget.BUYBOX)
        {
            int value = BuyValue;
            if (value > 0)
            {
                valueIcon.SetActive(true);
                valueText.text = value.ToString();
            }
            else
            {
                valueIcon.SetActive(false);
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (animating)
        {
            return;
        }

        if (dragItemsParent.childCount >= 10)
        {
            return;
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            if (draggableItem.moveTarget != localMoveTarget)
            {
                return;
            }

            hasItemsToReturn = true;

            draggableItem.moveTarget = returnMoveTarget;

            //draggableItem.foundContainer = true;
            draggableItem.originParent = dragItemsParent;
            draggableItem.transform.SetParent(dragItemsParent);

            draggableItem.handler.RemoveItem(draggableItem.item, true);
            hasItemsToReturn = true;
            if (draggableItem.parentButton != null)
            Destroy(draggableItem.parentButton);

            dropped.transform.SetAsLastSibling();
            
            handler.UpdateOfferButton();

            UpdateValueIcons();
        }
    }
}


public enum DraggedItemState
{
    ReturnToInventory,
    ReturnToStore,
    ReturnToParent
}
