using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 startPos;
    [SerializeField] Image raycastItem; 
    public Transform whileDragParent;
    public Transform originParent;
    public bool foundContainer;
    public Item item; //the button parent should set the item here 
    Vector3 currentPos;
    public GameObject parentButton;
    public DraggedItemState state = DraggedItemState.ReturnToParent;

    void Start()
    {
        raycastItem = this.GetComponent<Image>();
        originParent = this.transform.parent;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (foundContainer)
        {
            return;
        }
        transform.SetParent(whileDragParent);
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
        switch (state)
        {
            case DraggedItemState.ReturnToParent:
                if (!foundContainer)
                {
                    transform.SetParent(originParent);
                }
            break;
            case DraggedItemState.ReturnToStore:
                break;
            case DraggedItemState.ReturnToInventory:
                break;
        }

    }
}
