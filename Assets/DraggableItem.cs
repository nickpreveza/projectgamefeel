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

    public ItemMoveTarget moveTarget = ItemMoveTarget.INVALID;
    public InventoryManager handler;
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

        if (!foundContainer)
        {
            transform.SetParent(originParent);
            transform.parent.SetAsLastSibling();          
        }
        /*
        else
        {
            switch (state)
            {
                case DraggedItemState.ReturnToParent:

                    break;
                case DraggedItemState.ReturnToStore:
                    CityView.Instance.storeManager.AddItem();

                    break;
                case DraggedItemState.ReturnToInventory:
                    break;
            }
        }*/
       

    }
}

public enum ItemMoveTarget
{
    STORE,
    INVENTORY,
    BUYBOX,
    SELLBOX,
    INVALID
}
