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

    public bool subPanelOpen = false;
    public void StructureSelected(CityStructureType structureType)
    {
        if (subPanelOpen)
        {
            return;
        }
        subPanelOpen = true;
        switch (structureType)
        {
            case CityStructureType.LEADER:
                leaderScreen.SetActive(true);
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

    public void BackButton()
    {
        if (subPanelOpen)
        {
            CloseSubPanles();
            subPanelOpen = false;
        }
    }
    public void WarButton()
    {

    }

    public void TraderButton()
    {

    }

    public void SellButton()
    {

    }

    public void ShowInventory()
    {
        inventoryScreen.SetActive(true);
        inventoryScreen.GetComponent<InventoryManager>().ShowInventory();
    }

    public void CloseSubPanles()
    {
        leaderScreen.SetActive(false);
        questScreen.SetActive(false);
        armyScreen.SetActive(false);
        equipmentScreen.SetActive(false);

        inventoryScreen.SetActive(false);
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
    }

    public void UpdatePlayerStats()
    {
        playerGold.text = FeudGameManager.Instance.playerGold.ToString();
    }
    public void HideCity()
    {
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

