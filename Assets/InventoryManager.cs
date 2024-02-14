using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour, IDropHandler
{
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject buttonPrefab;

    public List<Item> itemsToDisplay = new List<Item>();
    [SerializeField] Transform canvasDraggableParent;
    public List<Item> refList = new List<Item>();

    public ItemMoveTarget parentType;
    public ItemMoveTarget sendTarget;

    [SerializeField] StoreManager handler;
    [SerializeField] ArmyManager armyHandler;

    [SerializeField] GameObject emptyDisclaimer;

    public bool armyMode;

    public void AddItem(Item item, bool setAsFirstSibling, bool refreshVisual)
    {
        if (setAsFirstSibling)
        {
            refList.Insert(0, item);
        }
        else
        {
            refList.Add(item);
        }

        if (refreshVisual)
        {
            ShowCollection(refList);
        }
       
    }

    public void RemoveItem(Item item, bool updateVisual)
    {
        if (refList.Contains(item))
        {
            refList.Remove(item);

            if (updateVisual)
            {
                ShowCollection(refList);
            }
           
        }
    }
    public void ShowCollection(List<Item> itemList)
    {
        ClearInventory();
        refList = itemList;
        itemsToDisplay = new List<Item>(itemList);
        if (itemsToDisplay.Count <= 0)
        {
            emptyDisclaimer?.SetActive(true);
        }
        else
        {
            emptyDisclaimer?.SetActive(false);
        }
        foreach (Item item in itemsToDisplay)
        {
            GameObject obj = Instantiate(buttonPrefab, contentParent);
            obj.GetComponent<InventoryItem>().SetData(this, item, canvasDraggableParent, armyMode);

        }
    }

    void ClearInventory()
    {
        refList = new List<Item>();

        foreach (Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        /*
        while (contentParent.childCount > 0)
        {
            Destroy(contentParent.GetChild(0).gameObject);
        }*/
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            if (draggableItem.GetSendType != parentType)
            {
                return;
            }

            AddItem(draggableItem.item, true, true);

            Destroy(draggableItem.gameObject);

            if (!armyMode)
            {
                handler.UpdateOfferButton();
            }
            else
            {
                armyHandler.UpdateBuyButton();
            }

           
        }
    }

}
