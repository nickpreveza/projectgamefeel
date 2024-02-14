using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AllIn1SpriteShader;
using ntw.CurvedTextMeshPro;

public class CityView : MonoBehaviour
{
    public static CityView Instance;

    public WorldCity selectedCity;
    [SerializeField] GameObject cityParent;
    [SerializeField] TextMeshProUGUI cityName;
    [SerializeField] TextMeshProUGUI playerGold;

    [SerializeField] CityStructure leaderButton;
    [SerializeField] CityStructure questsButton;
    [SerializeField] CityStructure shopButton;
    [SerializeField] CityStructure armyButton;

    [SerializeField] Image friendlinessFill;
    [SerializeField] Image powerFill;

    [SerializeField] Image civilizationIcon;

    [SerializeField] GameObject leaderScreen;
    [SerializeField] GameObject questScreen;
    [SerializeField] GameObject armyScreen;
    [SerializeField] GameObject shopScreen;
    [SerializeField] GameObject vecrticalInventory;

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
    public bool storeOpen = false;
    public bool armyOpen = false;

    [SerializeField] DragTargetSlot leaderGiftBox;
    [SerializeField] Image civColorInCityView;

    [SerializeField] GameObject leaderQuestionMark;
    [SerializeField] GameObject trustQuestionMark;

    [SerializeField] Image leaderSprite;

    private void Awake()
    {
        Instance = this;
    }

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
                if (selectedCity.civIndex == 0)
                {
                    leaderOwnedVersion.SetActive(true);
                 
                    subPanelOpen = true;
                }
                else
                {
                    leaderScreen.SetActive(true);
                    SetLeaderTextWelome();
                   
                    selectedCity.LeaderRevealed();
                    leaderImage.sprite = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderImage;
                }
          
                break;
            case CityStructureType.SHOP:
                shopScreen.SetActive(true);
                shopScreen.GetComponent<StoreManager>().ShowStore(this);
                storeOpen = true;
                break;
            case CityStructureType.QUESTS:
                questScreen.SetActive(true);
                break;
            case CityStructureType.ARMY:
                armyScreen.SetActive(true);
                armyScreen.GetComponent<ArmyManager>().ShowArmy();
                armyOpen = true;
                break;
        }
    }

    public void ItemGivenToLeader(Item item )
    {
        if (FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].wantedItemsTypes.Contains(item.type))
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderGiftΑccept;
        }
        else if(FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].hatedItemsTypes.Contains(item.type))
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderGiftDecline;
        }
        else
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderGiftDecline;
        }

        //send reward here 
    }

    public void SetLeaderTextTradingDefault()
    {
        leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderGiftPrompt;
    }

    public void BackButton()
    {
        if (tradingOpen)
        {
            HideTrading();
            SetLeaderTextWelome();
            return;
        }
        
        if (challengingOpen)
        {
            HideWar();
            SetLeaderTextWelome();
            return;
        }

        if (storeOpen)
        {
            shopScreen.GetComponent<StoreManager>().HideStore();
        }

        if (armyOpen)
        {
            armyScreen.GetComponent<ArmyManager>().HideArmy();
        }

        if (subPanelOpen)
        {
            CloseSubPanles();
            tradingOpen = false;
            subPanelOpen = false;
            storeOpen = false;
            armyOpen = false;
            ShowCity(selectedCity);
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
        leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderChallengePrompt;
    }
    public void HideWar()
    {
        warScreen.SetActive(false);
        challengingOpen = false;
    }

    public void SetLeaderTextWelome()
    {
        if (FeudGameManager.Instance.GetCiv(selectedCity.civIndex).trustforPlayer > 0.6)
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderWelcomePositive;
        }
        else if (FeudGameManager.Instance.GetCiv(selectedCity.civIndex).trustforPlayer > 0.3)
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderWelcomeNeutral;
        }
        else
        {
            leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderWelcomeNegative;
        }
    }

    public void EnterCombatButton()
    {
        FeudGameManager.Instance.StartArena(true, FeudGameManager.Instance.GetCiv(selectedCity.civIndex).formationUnits);
    }

    public void ShowTrading()
    {
        tradingOpen = true;
        leaderText.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderGiftPrompt;
        tradeBox.SetActive(true);
        leaderImage.GetComponent<Animator>().SetTrigger("toTrader");
        leaderGiftBox.SetCoverFillAmount(0f);
        ShowInventory();
    }

    public void HideTrading()
    {
        tradingOpen = false;
        SetLeaderTextWelome();
        tradeBox.SetActive(false);
        leaderImage.GetComponent<Animator>().SetTrigger("fromTrader");
        HideInventory();
    }

    public void ShowInventory()
    {
        inventoryScreen.SetActive(true);
        inventoryScreen.GetComponent<InventoryManager>().ShowCollection(FeudGameManager.Instance.Player().ownedItems);
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
        shopScreen.SetActive(false);
        warScreen.SetActive(false);
        leaderOwnedVersion.SetActive(false);
        vecrticalInventory.SetActive(false);
        if (shopScreen.activeSelf)
        {
            shopScreen.GetComponent<StoreManager>().HideStore();
        }
        shopScreen.SetActive(false);
        //leaderOwnedVersion.SetActive(false);
        inventoryScreen.SetActive(false);
        tradeBox.SetActive(false);
    }
    public void ShowCity(WorldCity _selectedCity)
    {
        selectedCity = _selectedCity;

        leaderButton.targetIconCanvasGroup.alpha = 0;
        questsButton.targetIconCanvasGroup.alpha = 0;
        shopButton.targetIconCanvasGroup.alpha = 0;
        armyButton.targetIconCanvasGroup.alpha = 0;
      
        friendlinessFill.fillAmount = FeudGameManager.Instance.GetCiv(selectedCity.civIndex).trustforPlayer;
        //powerFill.fillAmount = selectedCity.powerLevel;

        cityName.text = selectedCity.cityName;
        cityName.GetComponent<TextProOnACircle>().m_forceUpdate = true;
        leaderName.text = FeudGameManager.Instance.gameCivilizations[selectedCity.civIndex].leaderName;
        cityParent.SetActive(true);

        UpdatePlayerStats();

        selectedCity.RevealCity();

        if (!selectedCity.hasPlayerSeenLeader)
        {

            if (selectedCity.civIndex == 0)
            {
                leaderQuestionMark.SetActive(false);
                trustQuestionMark.SetActive(false);
                civColorInCityView.color = FeudGameManager.Instance.GetCiv(selectedCity.civIndex).mainColor;
                SetLeaderSpriteColor(FeudGameManager.Instance.GetCiv(selectedCity.civIndex).mainColor);
                selectedCity.hasPlayerSeenLeader = true;
            }
            else
            {
                leaderQuestionMark.SetActive(true);
                trustQuestionMark.SetActive(true);
                civColorInCityView.color = Color.white;
            }
           

            
        }
        else
        {
            leaderQuestionMark.SetActive(false);
            trustQuestionMark.SetActive(false);
            civColorInCityView.color = FeudGameManager.Instance.GetCiv(selectedCity.civIndex).mainColor;
            SetLeaderSpriteColor(FeudGameManager.Instance.GetCiv(selectedCity.civIndex).mainColor);

        }

        CloseSubPanles();
        subPanelOpen = false;
        tradingOpen = false;
    }

    public void SetLeaderSpriteColor(Color color)
    {
        Material material = leaderSprite.material;
        material.SetColor("_ColorChangeNewCol", color);
        material.SetColor("_ColorChangeNewCol2", color);
        leaderSprite.material = material;
    }

    public void UpdatePlayerStats()
    {
        playerGold.text = FeudGameManager.Instance.Player().gold.ToString();
    }
    public void HideCity()
    {
        subPanelOpen = false;
        tradingOpen = false;
        challengingOpen = false;
        cityParent.SetActive(false);
    }

    public void HideCityButton()
    {
        SI_CameraController.Instance.HideCity();
        HideCity();
    }
}

[System.Serializable]
public class crossworldUI
{
    public GameObject background;
    public GameObject iconHolder;
    public Image image;
}

