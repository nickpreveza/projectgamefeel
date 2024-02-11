using UnityEngine;
using UnityEngine.EventSystems;

public class DragTargetSlot : MonoBehaviour, IDropHandler
{
   public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.foundContainer = true;
            draggableItem.dragParent = this.transform;
            draggableItem.transform.SetParent(this.transform);
        }
   }
}
