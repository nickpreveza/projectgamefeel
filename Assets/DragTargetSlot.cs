using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class DragTargetSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] CityView handler;
    GameObject parentButton;
    GameObject movedIcon;
    [SerializeField] Image boxCover; 
    Item itemData = new Item();
    bool animating;
    public float lidSpeed = 1f;

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
            animating = true;
            movedIcon = dropped;
            draggableItem.foundContainer = true;
            draggableItem.transform.SetParent(this.transform);
            itemData = draggableItem.item;
            parentButton = draggableItem.parentButton;
            boxCover.transform.SetAsLastSibling();

            FeudGameManager.Instance.Player().ownedItems.Remove(draggableItem.item);
            StartCoroutine(SendItemCoroutine());
        }
   }

    public void SetCoverFillAmount(float amount)
    {
        boxCover.fillAmount = amount;
    }

    IEnumerator SendItemCoroutine()
    {
        boxCover.fillAmount = 0;
        Destroy(parentButton);
        while (boxCover.fillAmount < 1)
        {
            boxCover.fillAmount += 0.05f * lidSpeed;
            yield return new WaitForFixedUpdate();
        }
        Destroy(movedIcon);
        handler.ItemGivenToLeader(itemData);

        yield return new WaitForSeconds(2f);
        while (boxCover.fillAmount > 0)
        {
            boxCover.fillAmount -= 0.1f * lidSpeed;
            yield return new WaitForFixedUpdate();
        }

       
        animating = false;
        handler.SetLeaderTextTradingDefault();
    }

}
