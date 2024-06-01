using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{

    public class DropPlaceScr : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            Card card = eventData.pointerDrag.GetComponent<Card>();

            if (card != null)
            {
                card.defaultParent = transform;

                Vector3 cardOffset = card.transform.position;
                cardOffset.z -= 1f;
                card.transform.position = cardOffset;

            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag ==null) { return; }

            Card card = eventData.pointerDrag.GetComponent<Card>();

            if(card)
                card.defaultTempCardParent = transform;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) { return; }

            Card card = eventData.pointerDrag.GetComponent<Card>();

            if (card && card.defaultTempCardParent == transform)
                card.defaultTempCardParent = card.defaultParent;
        }
    }
}
