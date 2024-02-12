using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityView : MonoBehaviour
{

    public WorldCity selectedCity;
    [SerializeField] GameObject cityParent;
    [SerializeField] TextMeshProUGUI cityName;
    [SerializeField] TextMeshProUGUI playerGold;

    [SerializeField] CityStructure leader;
    [SerializeField] CityStructure quests;
    [SerializeField] CityStructure equipment;
    [SerializeField] CityStructure army;

    [SerializeField] Image friendlinessFill;
    [SerializeField] Image powerFill;

    [SerializeField] Image civilizationIcon;

    [SerializeField] GameObject leaderScreen;
    [SerializeField] GameObject questScreen;
    [SerializeField] GameObject armyScreen;
    [SerializeField] GameObject equipmentScreen;

    [SerializeField] GameObject inventoryScreen;

    [SerializeField] TextMeshProUGUI leaderName;
    [SerializeField] TextMeshProUGUI leaderText;
    [SerializeField] Image leaderImage;
    [SerializeField] GameObject tradeBox;

    [SerializeField] GameObject warScreen;


    [SerializeField] GameObject leaderOwnedVersion;
    [SerializeField] GameObject combatResultsScreen;

    public bool subPanelOpen = false;
    public bool tradingOpen = false;
    public bool challengingOpen = false;
    public void StructureSelected(CityStructureType structureType)
    {

        if (subPanelOpen)
        {
            return;
        }

        CloseSubPanles();
        challengingOpen = false;
        subPanelOpen = true;
        switch (structureType)
        {
            case CityStructureType.LEADER:
                leaderScreen.SetActive(true);
                leaderText.text = "I am the law.";
                break;
            case CityStructureType.EQUIPMENT:
                equipmentScreen.SetActive(true);
                break;
            case CityStructureType.QUESTS:
                questScreen.SetActive(true);
                break;
            case CityStructureType.ARMY:
                armyScreen.SetActive(true);
                break;
        }
    }

    public void ItemGivenToLeader(Item item )
    {
        Debug.Log("Item consumed");
    }
    public void BackButton()
    {
        if (tradingOpen)
        {
            HideTrading();
            return;
        }
        
        if (challengingOpen)
        {
            HideWar();
            return;
        }

        if (subPanelOpen)
        {
            CloseSubPanles();
            tradingOpen = false;
            subPanelOpen = false;
        }
    }

    public void UpdateCombatResults()
    {
        //TODO show a subpanel with the results of combat, update necessary stats 
    }
    public void WarButton()
    {
        warScreen.SetActive(true);
        challengingOpen = true;
        leaderText.text = "I'm not sure you can do that.";
    }
    public void HideWar()
    {
        warScreen.SetActive(false);
        challengingOpen = false;
    }

    public void EnterCombatButton()
    {
        FeudGameManager.Instance.StartArena(true);
    }

    public void ShowTrading()
    {
        tradingOpen = true;
        leaderText.text = "I'm looking for a keyword";
        tradeBox.SetActive(true);
        leaderImage.GetComponent<Animator>().SetTrigger("toTrader");
        ShowInventory();
    }

    public void HideTrading()
    {
        tradingOpen = false;
        leaderText.text = "I am the law";
        tradeBox.SetActive(false);
        leaderImage.GetComponent<Animator>().SetTrigger("fromTrader");
        HideInventory();
    }

    public void ShowInventory()
    {
        inventoryScreen.SetActive(true);
        inventoryScreen.GetComponent<InventoryManager>().ShowInventory();
    }

    public void HideInventory()
    {
        inventoryScreen.SetActive(false);
    }

    public void CloseSubPanles()
    {
        leaderScreen.SetActive(false);
        questScreen.SetActive(false);
        armyScreen.SetActive(false);
        equipmentScreen.SetActive(false);
        warScreen.SetActive(false);
        //leaderOwnedVersion.SetActive(false);
        inventoryScreen.SetActive(false);
        tradeBox.SetActive(false);
    }
    public void ShowCity(WorldCity _selectedCity)
    {
        selectedCity = _selectedCity;

        leader.targetIconCanvasGroup.alpha = 0;
        quests.targetIconCanvasGroup.alpha = 0;
        equipment.targetIconCanvasGroup.alpha = 0;
        army.targetIconCanvasGroup.alpha = 0;

        friendlinessFill.fillAmount = selectedCity.friendlinessLevel;
        powerFill.fillAmount = selectedCity.powerLevel;

        cityName.text = selectedCity.cityName;
        cityParent.SetActive(true);

        UpdatePlayerStats();

        selectedCity.RevealCity();
        CloseSubPanles();
        subPanelOpen = false;
        tradingOpen = false;
    }

    public void UpdatePlayerStats()
    {
        playerGold.text = FeudGameManager.Instance.playerGold.ToString();
    }
    public void HideCity()
    {
        subPanelOpen = false;
        tradingOpen = false;
        challengingOpen = false;
        cityParent.SetActive(false);
    }
}

[System.Serializable]
public class crossworldUI
{
    public GameObject background;
    public GameObject iconHolder;
    public Image image;
}

