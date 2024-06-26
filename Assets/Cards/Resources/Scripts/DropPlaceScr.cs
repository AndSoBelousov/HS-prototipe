using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{

    public class DropPlaceScr : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private CardManager cardManager;

        public FieldType fieldType;

        private void Start()
        {
            cardManager = FindFirstObjectByType<CardManager>(); // Находим CardManager в сцене
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (fieldType != FieldType.SelfField) { return; }

            Card card = eventData.pointerDrag.GetComponent<Card>();

            if (card != null)
            {
                card.defaultParent = transform;

                Vector3 cardOffset = card.transform.position;
                cardOffset.z -= 1f;
                card.transform.position = cardOffset;

                // Если карта перемещена на стол, уведомляем CardManager
                if (this.fieldType == FieldType.SelfField || this.fieldType == FieldType.EnemyField)
                {
                    bool isPlayer = this.fieldType == FieldType.SelfField;
                    cardManager.OnCardMovedToField(card, isPlayer);

                    //cardManager.InformationAboutTheLista();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag ==null || fieldType == FieldType.EnemyField
                || fieldType == FieldType.EnemyHand) { return; }

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
