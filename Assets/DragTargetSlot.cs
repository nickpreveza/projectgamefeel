using UnityEngine;
using UnityEngine.EventSystems;

public class DragTargetSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] CityView handler;
    GameObject parentButton;
    GameObject movedIcon;
    Item itemData = new Item();
   public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            movedIcon = dropped;
            draggableItem.foundContainer = true;
            draggableItem.dragParent = this.transform;
            draggableItem.transform.SetParent(this.transform);
            itemData = draggableItem.item;
            parentButton = draggableItem.parentButton;
            Invoke("RegisterAndDestroyItem", 1f);
        }
   }

    void RegisterAndDestroyItem()
    {
        handler.ItemGivenToLeader(itemData);

        Destroy(movedIcon);
        Destroy(parentButton);
    }
}
