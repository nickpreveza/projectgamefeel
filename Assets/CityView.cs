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

