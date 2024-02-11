using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 startPos;
    [SerializeField] Image raycastItem; 
    public Transform dragParent;
    public Transform originParent;
    public bool foundContainer;

    Vector3 currentPos;

    void Start()
    {
        raycastItem = this.GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (foundContainer)
        {
            return;
        }
        transform.SetParent(dragParent);
        transform.SetAsLastSibling();

        raycastItem.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (foundContainer)
        {
            return;
        }
        currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
        currentPos.z = 0;
        transform.position = currentPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        raycastItem.raycastTarget = true;
        //check
        if (!foundContainer)
        {
            transform.SetParent(originParent);
        }
    }
}
