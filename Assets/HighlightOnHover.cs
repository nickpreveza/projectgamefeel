using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class HighlightOnHover : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool animating;
    bool fadingIn;
    bool fadingOut;
    [SerializeField] CanvasGroup targetCanvasGroup;
    [SerializeField] float fadingSpeed = 1;

    void Start()
    {
        targetCanvasGroup.alpha = 0;
    }
    private void Update()
    {
        if (fadingIn)
        {
            if (targetCanvasGroup.alpha < 1)
            {
                targetCanvasGroup.alpha += 0.1f * fadingSpeed;
            }
            else
            {
                fadingIn = false;
            }
        }
        else if (fadingOut)
        {
            if (targetCanvasGroup.alpha > 0)
            {
                targetCanvasGroup.alpha -= 0.1f * fadingSpeed;
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
