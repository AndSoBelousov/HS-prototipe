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
            cardManager = FindFirstObjectByType<CardManager>(); // Ќаходим CardManager в сцене
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("отпустил");
            if (fieldType != FieldType.SelfField) { return; }

            Card card = eventData.pointerDrag.GetComponent<Card>();

            //cardManager.FindId((int)card.IdCard);
            Debug.Log("карта прошла проверку пол€");

            if (card != null && cardManager.playerFieldList.Count < 6)
            {
                card.defaultParent = transform;
                Debug.Log("прошла проверку колличества");
                Vector3 cardOffset = card.transform.position;
                cardOffset.z = -0.2f;
                card.transform.position = cardOffset;
                 
                // ≈сли карта перемещена на стол, уведомл€ем CardManager
                if (this.fieldType == FieldType.SelfField || this.fieldType == FieldType.EnemyField)
                {
                    Debug.Log("карта перемещена");
                    bool isPlayer = this.fieldType == FieldType.SelfField;
                    cardManager.OnCardMovedToField(card, isPlayer);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("навел мышь");
            if (eventData.pointerDrag ==null || fieldType == FieldType.EnemyField
                || fieldType == FieldType.EnemyHand || fieldType == FieldType.SelfHand ) { return; }

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
