using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DragStoreTarget : MonoBehaviour, IDropHandler
{
    bool animating;
    public bool armyMode;
    public bool allowSwitch;

    public ItemMoveTarget parentType;
    public ItemMoveTarget sendTarget;
    //public ItemMoveTarget acceptTarget;

    [SerializeField] Transform dragItemsParent;
    [SerializeField] int maxItemsInContainer = 10;
    public bool hasItemsToReturn;

    [SerializeField] InventoryManager storeItems;
    [SerializeField] InventoryManager userItems;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] GameObject valueIcon;
    public StoreManager handler;
    public ArmyManager armyHandler;

    public GameObject holdingItem;
    
    public DraggableItem unitDraggable
    {
        get
        {
            if (dragItemsParent.transform.childCount > 0)
            {
                return dragItemsParent.GetChild(0).GetComponent<DraggableItem>();
            }
            else
            {
                return null;
            }
         
        }
    }

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
        if (armyMode)
        {
            return;
        }

        if (parentType == ItemMoveTarget.SELLBOX)
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
        else if (parentType == ItemMoveTarget.BUYBOX)
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

    public void ScriptDrop(GameObject prefab, Item item, Transform dragParent)
    {
        if (dragItemsParent.childCount >= maxItemsInContainer)
        {
            foreach(Transform child in dragItemsParent)
            {
                Destroy(child);
            }
        }


        GameObject dropped = Instantiate(prefab, dragItemsParent);
        holdingItem = dropped;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        draggableItem.SetUpUnit(item, dragParent, parentType, sendTarget);
        draggableItem.originParent = dragItemsParent;
        draggableItem.transform.SetParent(dragItemsParent);

        if (draggableItem.handler != null)
        {
            draggableItem.handler.RemoveItem(draggableItem.item, true);
        }

        if (draggableItem.parentButton != null)
        {
            Destroy(draggableItem.parentButton);
        }

        dropped.transform.SetAsLastSibling();
        armyHandler.UpdateBuyButton();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (animating)
        {
            return;
        }

        if (dragItemsParent.childCount >= maxItemsInContainer)
        {
            if (!allowSwitch)
            {
                return;
            }
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            if (draggableItem.GetSendType == ItemMoveTarget.STORE && parentType == ItemMoveTarget.BUYBOX)
            {
                //here should be function to allow switch between buy boxes
            }

            if (draggableItem.GetSendType != parentType)
            {
                return;
            }

            if (armyMode)
            {
                draggableItem.GetComponent<RectTransform>().localScale = Vector3.one;

                if (parentType == ItemMoveTarget.STORAGE && allowSwitch && dragItemsParent.childCount > 0  && draggableItem.GetSendType == parentType)
                {
                    DraggableItem localDraggable = dragItemsParent.GetChild(0).GetComponent<DraggableItem>();

                    if (localDraggable != null && localDraggable != draggableItem && localDraggable.GetSendType == ItemMoveTarget.STORAGE)
                    {
                        Transform parentToSwitch = draggableItem.originParent;
                        draggableItem.ForceMove(dragItemsParent);
                        localDraggable.ForceMove(parentToSwitch);
                    }


                    return;
                }
        
                if (draggableItem.GetParentType == ItemMoveTarget.STORE)
                {
                    hasItemsToReturn = true;

                    //uncsry
                    sendTarget = ItemMoveTarget.STORE;
                    draggableItem.handler.RemoveItem(draggableItem.item, true);
                }
                else
                {
                    hasItemsToReturn = false;
                    //sendTarget = ItemMoveTarget.BUYBOX;
                }

                draggableItem.SetUpOriginAndTarget(parentType, sendTarget);

                draggableItem.originParent = dragItemsParent;
                draggableItem.transform.SetParent(dragItemsParent);

                if (draggableItem.parentButton != null)
                {
                    Destroy(draggableItem.parentButton);
                }

                dropped.transform.SetAsLastSibling();

                armyHandler.UpdateBuyButton();

            }
            else
            {
                hasItemsToReturn = true;

                draggableItem.SetUpOriginAndTarget(parentType, sendTarget);

                //draggableItem.foundContainer = true;
                draggableItem.originParent = dragItemsParent;
                draggableItem.transform.SetParent(dragItemsParent);

                draggableItem.handler.RemoveItem(draggableItem.item, true);
               
                if (draggableItem.parentButton != null)
                    Destroy(draggableItem.parentButton);

                dropped.transform.SetAsLastSibling();

                handler.UpdateOfferButton();

                UpdateValueIcons();
            }
          
        }
    }
}

