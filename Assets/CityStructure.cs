using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CityStructure : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup targetIconCanvasGroup;
    public CityStructureType type;

    public bool animating;
    bool fadingIn;
    bool fadingOut;
    [SerializeField] float fadingSpeed = 1;
    [SerializeField] CityView handler;

    private void Update()
    {
        if (fadingIn)
        {
            if (targetIconCanvasGroup.alpha < 1)
            {
                targetIconCanvasGroup.alpha += 0.1f * fadingSpeed;
            }
            else
            {
                fadingIn = false;
            }
        }
        else if (fadingOut)
        {
            if (targetIconCanvasGroup.alpha > 0)
            {
                targetIconCanvasGroup.alpha -= 0.1f * fadingSpeed;
            }
            else
            {
                fadingOut = false;
   
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fadingOut)
        {
            fadingOut = false;
        }
        fadingIn = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        handler.StructureSelected(type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (fadingIn)
        {
            fadingIn = false;
        }

        fadingOut = true;
    }
}

public enum CityStructureType
{
    LEADER,
    ARMY,
    EQUIPMENT,
    QUESTS
}