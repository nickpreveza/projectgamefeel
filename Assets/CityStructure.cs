using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CityStructure : MonoBehaviour
{
    public GameObject background;
    public GameObject iconHolder;
    public CanvasGroup targetIconCanvasGroup;
    public CityStructureType type;

    public bool animating;
    bool fadingIn;
    bool fadingOut;
    [SerializeField] float fadingSpeed = 1;
    [SerializeField] CityView handler;

    void OnMouseEnter()
    {
        if (fadingOut)
        {
            fadingOut = false;
        }
        fadingIn = true;
       // targetIconCanvasGroup.alpha = 1;
    }

    // ...the red fades out to cyan as the mouse is held over...
    void OnMouseOver()
    {
        //targetIconCanvasGroup.alpha = 1;
    }

    private void OnMouseDown()
    {
        if (IsPointerOverUIObject())
        {

        }

        handler.StructureSelected(type);
    }

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit()
    {
        if (fadingIn)
        {
            fadingIn = false;
        }

        fadingOut = true; 
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }


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

}

public enum CityStructureType
{
    LEADER,
    ARMY,
    EQUIPMENT,
    QUESTS
}