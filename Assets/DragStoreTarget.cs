using UnityEngine;
using UnityEngine.EventSystems;

public class DragStoreTarget : MonoBehaviour, IDropHandler
{
    bool animating;
    GameObject movedIcon;
    public DragTargetType dragTargetType;
    [SerializeField] Transform dragItemsParent;

    public void OnDrop(PointerEventData eventData)
    {
        if (animating)
        {
            return;
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            
            movedIcon = dropped;

            switch(dragTargetType)
            {
                case DragTargetType.ItemsToSell:
                    draggableItem.state = DraggedItemState.ReturnToInventory;
                    break;
                case DragTargetType.ItemsToBuy:
                    draggableItem.state = DraggedItemState.ReturnToStore;
                    break;
            }

           
            //draggableItem.foundContainer = true;
            draggableItem.originParent = this.transform;
            draggableItem.transform.SetParent(this.transform);

            if (draggableItem.parentButton != null)
            Destroy(draggableItem.parentButton);

            movedIcon.transform.SetAsLastSibling();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum DragTargetType
{
    ItemsToSell,
    ItemsToBuy
}

public enum DraggedItemState
{
    ReturnToInventory,
    ReturnToStore,
    ReturnToParent
}
