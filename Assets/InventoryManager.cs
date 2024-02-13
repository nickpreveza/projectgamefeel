using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Transform dragParentToAssign;
    public List<Item> itemsToDisplay = new List<Item>();
    [SerializeField] Transform canvasDraggableParent;

    public void ShowInventory()
    {
        ClearInventory();

        itemsToDisplay = new List<Item>(FeudGameManager.Instance.Player().ownedItems);
        foreach(Item item in itemsToDisplay)
        {
            Instantiate(buttonPrefab, contentParent);
            buttonPrefab.GetComponent<InventoryItem>().SetData(item, canvasDraggableParent);
        }
        //foreach item in player 
        //create an inventory button 
        //set up each button to have the leader's inventory box as target 
    }

    void ClearInventory()
    {
        foreach(Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void HideInventory()
    {

    }

    public void OnDragCompleted()
    {

    }


}
