using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityView : MonoBehaviour
{

    public WorldCity selectedCity;
    [SerializeField] GameObject cityParent;
    //barracks
    //tavern
    //forge
    public void ShowCity()
    {
        cityParent.SetActive(true);
    }
    public void HideCity()
    {
        cityParent.SetActive(false);
    }
}
