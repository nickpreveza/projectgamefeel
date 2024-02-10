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

    [SerializeField] CityStructure leader;
    [SerializeField] CityStructure quests;
    [SerializeField] CityStructure equipment;
    [SerializeField] CityStructure army;

    public void ShowCity(WorldCity _selectedCity)
    {
        leader.targetIconCanvasGroup.alpha = 0;
        quests.targetIconCanvasGroup.alpha = 0;
        equipment.targetIconCanvasGroup.alpha = 0;
        army.targetIconCanvasGroup.alpha = 0;


        selectedCity = _selectedCity;
        cityName.text = selectedCity.cityName;
        cityParent.SetActive(true);
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

