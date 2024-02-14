using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;

public class ArmyManager : MonoBehaviour
{
    [SerializeField] InventoryManager unitShop;

    [SerializeField] DragStoreTarget[] unitBuyBoxPositions;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI buyButtonText;
    public List<Item> editedPlayerUnits = new List<Item>();
    [SerializeField] GameObject draggableUnitPrefab;
    [SerializeField] Transform canvasDragParent;
    public void ShowFormation()
    {
        editedPlayerUnits = FeudGameManager.Instance.Player().formationUnits;

        foreach (DragStoreTarget target in unitBuyBoxPositions)
        {
           
            target.allowSwitch = true;
            target.armyHandler = this;
            target.armyMode = true;
            target.hasItemsToReturn = false;
            target.ClearBox();
        }

        for (int i = 0; i < editedPlayerUnits.Count; i++)
        {
            Item item = editedPlayerUnits[i];

            if (item.invalidated)
            {
                unitBuyBoxPositions[i].parentType = ItemMoveTarget.BUYBOX;
                unitBuyBoxPositions[i].sendTarget = ItemMoveTarget.STORE;
                Color color = unitBuyBoxPositions[i].gameObject.GetComponent<Image>().color;
                color.a = 1f;
                unitBuyBoxPositions[i].gameObject.GetComponent<Image>().color = color;
            }
            else
            {
                unitBuyBoxPositions[i].parentType = ItemMoveTarget.STORAGE;
                unitBuyBoxPositions[i].sendTarget = ItemMoveTarget.STORAGE;

                unitBuyBoxPositions[i].ScriptDrop(draggableUnitPrefab, item, canvasDragParent);

                Color color = unitBuyBoxPositions[i].gameObject.GetComponent<Image>().color;
                color.a = 0.5f;
                unitBuyBoxPositions[i].gameObject.GetComponent<Image>().color = color;
            }
        }

        UpdateBuyButton();

    }


    public void UpdateBuyButton()
    {
        buyButton.interactable = false;
        int totalValue = 0;
        foreach (DragStoreTarget buybox in unitBuyBoxPositions)
        {
           
            if (buybox.hasItemsToReturn)
            {
                totalValue += buybox.BuyValue;
            }
        }

        if (totalValue > 0)
        {
           
            buyButtonText.text = "BUY UNITS (" + totalValue + "G)";

            if (FeudGameManager.Instance.CanPlayerAfford(totalValue)) 
            {
                buyButton.interactable = true;
            }
        }
        else
        {
            buyButton.interactable = false;
            buyButtonText.text = "DRAG UNITS TO BUY";
        }

    }

    public void ConfirmPurchase()
    {
        int totalValue = 0;
        foreach (DragStoreTarget buybox in unitBuyBoxPositions)
        {
            if (buybox.hasItemsToReturn)
            {
                totalValue += buybox.BuyValue;
            }
        }

        if (FeudGameManager.Instance.TryToChargePlayer(totalValue))
        {
            Debug.Log("Purchase complete");

            for (int i = 0; i < editedPlayerUnits.Count; i++)
            {
                if (unitBuyBoxPositions[i].unitDraggable != null)
                {
                    editedPlayerUnits[i] = unitBuyBoxPositions[i].unitDraggable.item;
                }
                else
                {
                    editedPlayerUnits[i].invalidated = true;
                }
              
            }

            FeudGameManager.Instance.Player().formationUnits = new List<Item>(editedPlayerUnits);
            ShowArmy();
        }
        else
        {
            Debug.LogWarning("Player couldn't afford purchase, but button hadn't been updated");
        }
    }

    public void ShowArmy()
    {
        unitShop.ShowCollection(FeudGameManager.Instance.GetCiv(CityView.Instance.selectedCity.civIndex).selectedStoreUnits);
        ShowFormation();
    }

    public void HideArmy()
    {     
        ClearBoxes();
    }

    void ClearBoxes()
    {
        foreach(DragStoreTarget target in unitBuyBoxPositions)
        {
            target.ClearBox();
        }
    }
}
