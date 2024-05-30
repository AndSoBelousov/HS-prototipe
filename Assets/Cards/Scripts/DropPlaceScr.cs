using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{

    public class DropPlaceScr : MonoBehaviour, IDropHandler
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
    }
}
