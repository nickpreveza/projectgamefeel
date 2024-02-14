using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 startPos;
    [SerializeField] Image raycastItem;
    [SerializeField] Image weaponImage;
    [SerializeField] Image shieldImage;
    public Transform whileDragParent;
    public Transform originParent;
    public bool foundContainer;
    public Item item; //the button parent should set the item here 
    Vector3 currentPos;
    public GameObject parentButton;

   
    public InventoryManager handler;

    ItemMoveTarget whereItIsComingFrom = ItemMoveTarget.INVALID;
    ItemMoveTarget whereItCanGo = ItemMoveTarget.INVALID;

    public ItemMoveTarget GetParentType
    {
        get
        {
            return whereItIsComingFrom;
        }
    }

    public ItemMoveTarget GetSendType
    {
        get
        {
            return whereItCanGo;
        }
    }
    void Start()
    {
        raycastItem = this.GetComponent<Image>();
        originParent = this.transform.parent;
    }

    public void SetUpOriginAndTarget(ItemMoveTarget newOrigin, ItemMoveTarget newTarget)
    {
        whereItIsComingFrom = newOrigin;
        whereItCanGo = newTarget;
    }

    public bool DoesItemOriginMatch(ItemMoveTarget targetAcceptType)
    {
        if (targetAcceptType != whereItIsComingFrom)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool DoesItemTargetMatch(ItemMoveTarget parentTypeHere)
    {
        if (parentTypeHere != whereItCanGo)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetUpUnit(Item _item, Transform _dragParent, ItemMoveTarget whereItsComing, ItemMoveTarget whereItsGoing)
    {
        item = _item;
        whileDragParent  = _dragParent;
        whereItCanGo = whereItsGoing;
        whereItIsComingFrom = whereItsComing;
        raycastItem.sprite = item.icon;
        weaponImage.raycastTarget = false;
        shieldImage.raycastTarget = false;

        if (item.weapon != null)
        {
            weaponImage.gameObject.SetActive(true);
            weaponImage.sprite = item.weapon.icon;
        }
        else
        {
            weaponImage.gameObject.SetActive(false);
        }

        if (item.shield != null)
        {
            shieldImage.gameObject.SetActive(true);
            shieldImage.sprite = item.shield.icon;
        }
        else
        {
            shieldImage.gameObject.SetActive(false);
        }

       
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

    public void ForceMove(Transform newParent)
    {
        originParent = newParent;
        raycastItem.raycastTarget = true;

        transform.SetParent(originParent);
        transform.parent.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        raycastItem.raycastTarget = true;

        if (!foundContainer)
        {
            transform.SetParent(originParent);
            transform.parent.SetAsLastSibling();          
        }

    }
}

public enum ItemMoveTarget
{
    STORE,
    INVENTORY,
    BUYBOX,
    SELLBOX,
    INVALID,
    STORAGE
}
