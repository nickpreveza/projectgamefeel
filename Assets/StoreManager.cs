using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [SerializeField] InventoryManager userInventory;
    [SerializeField] InventoryManager storeInventory;

    [SerializeField] DragStoreTarget sellTarget;
    [SerializeField] DragStoreTarget buyTarget;

    CityView handler;

    public int sellTargetValue;
    public int buyBoxValue;

    public int sellBoxHasObject;
    public int buyBoxHasObject;

    [SerializeField] Button makeOffer;
    [SerializeField] TextMeshProUGUI subtitleButton;

    [SerializeField] string defaultSubText;
    [SerializeField] string negativeResponseToOffer;
    [SerializeField] string positiveResponseToOffer;

    public void ShowStore(CityView _handler)
    {
        handler = _handler;
        userInventory.gameObject.SetActive(true);

        
        userInventory.ShowCollection(FeudGameManager.Instance.Player().ownedItems);
        storeInventory.ShowCollection(FeudGameManager.Instance.GetCiv(handler.selectedCity.civIndex).selectedStoreItems);

        sellTarget.handler = this;
        buyTarget.handler = this;
        sellTarget.hasItemsToReturn = false;
        buyTarget.hasItemsToReturn = false;
        sellTarget.ClearBox();
        buyTarget.ClearBox();

        subtitleButton.text = defaultSubText;
        UpdateOfferButton();
    }

    public void UpdateOfferButton()
    {
        sellTarget.UpdateValueIcons();
        buyTarget.UpdateValueIcons();

        int cost = buyTarget.BuyValue;

        if (sellTarget.SellValue > 0 && FeudGameManager.Instance.CanPlayerAfford(cost))
        {
            makeOffer.interactable = true;
        }
        else
        {
            makeOffer.interactable = false;
        }
    }

    public void MakeOffer()
    {
        if (sellTarget.SellValue >= buyTarget.BuyValue) //something something about trust 
        {
            if (FeudGameManager.Instance.TryToChargePlayer(buyTarget.BuyValue))
            {
                Debug.Log("Success!");
                buyTarget.AddItemsToUserInventory();
                sellTarget.RemoveItemsFromUserInventory();
                sellTarget.ClearBox();
                buyTarget.ClearBox();
                subtitleButton.text = positiveResponseToOffer;

                userInventory.ShowCollection(FeudGameManager.Instance.Player().ownedItems);
            }
            else
            {
                subtitleButton.text = "You don't have enough gold";
            }
        }
        else
        {
            subtitleButton.text = negativeResponseToOffer;
        }
    }

    public void HideStore()
    {
        userInventory.gameObject.SetActive(false);
        sellTarget.ClearBox();
        buyTarget.ClearBox();
    }
}
