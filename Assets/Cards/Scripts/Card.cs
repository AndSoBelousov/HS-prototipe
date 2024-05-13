using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace Cards
{
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private GameObject _frontCard;
        [SerializeField]
        private MeshRenderer _picture;
        [SerializeField]
        private TextMeshPro _cost;
        [SerializeField]
        private TextMeshPro _name;
        [SerializeField]
        private TextMeshPro _description;
        [SerializeField]
        private TextMeshPro _attack;
        [SerializeField]
        private TextMeshPro _type;
        [SerializeField]
        private TextMeshPro _health;

        private Canvas canvas;
        private Vector3 offset;

        [Header("Movement")]
        [SerializeField] private float moveSpeedLimit = 50;

        [Header("Selection")]
        public bool selected;
        public float selectionOffset = 50;
        private float pointerDownTime;
        private float pointerUpTime;

        [Header("States")]
        public bool isHovering;
        public bool isDragging;
        [HideInInspector] public bool wasDragged;

        [Header("Events")]
        [HideInInspector] public UnityEvent<Card> PointerEnterEvent;        //событие, которое вызываетс€ при наведении курсора на карточку.
        [HideInInspector] public UnityEvent<Card> PointerExitEvent;         //событие, которое вызываетс€ при уходе курсора с карточки.
        [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;     //событие, которое вызываетс€ при отпускании кнопки мыши на карточке. ¬ данном случае также передаетс€ логическое значение.
        [HideInInspector] public UnityEvent<Card> PointerDownEvent;         //событие, которое вызываетс€ при нажатии кнопки мыши на карточке.
        [HideInInspector] public UnityEvent<Card> BeginDragEvent;           //событие, которое вызываетс€ при начале перетаскивани€ карточки.
        [HideInInspector] public UnityEvent<Card> EndDragEvent;             //событие, которое вызываетс€ при завершении перетаскивани€ карточки.
        [HideInInspector] public UnityEvent<Card, bool> SelectEvent;        //событие, которое вызываетс€ при выборе карточки. ¬ данном случае также передаетс€ логическое значение.

        public bool IsFrontSide => _frontCard.activeSelf;

        public CardStateType State { get;  set; }


        public void Configuration(Material picture, CardPropertiesData data, string description )
        {
            _picture.sharedMaterial = picture;
            _cost.text = data.Cost.ToString();
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _type.text = data.Type == CardUnitType.None ? string.Empty : data.Type.ToString();// если типа нет то пусто, если есть то указываетс€ какой
            _health.text = data.Health.ToString();
        }

        void Update()
        {
            ClampPosition();

            if (isDragging)
            {
                Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
                transform.Translate(velocity * Time.deltaTime);
            }
        }

        void ClampPosition()
        {
            Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
            transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragEvent.Invoke(this);
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)transform.position;
            isDragging = true;
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
           
            wasDragged = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDragEvent.Invoke(this);
            isDragging = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;


            StartCoroutine(FrameWait());

            IEnumerator FrameWait()
            {
                yield return new WaitForEndOfFrame();
                wasDragged = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnterEvent.Invoke(this);
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExitEvent.Invoke(this);
            isHovering = false;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            PointerDownEvent.Invoke(this);
            pointerDownTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            pointerUpTime = Time.time;

            PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

            if (pointerUpTime - pointerDownTime > .2f)
                return;

            if (wasDragged)
                return;

            selected = !selected;
            SelectEvent.Invoke(this, selected);

            if (selected)
                transform.localPosition += (transform.up * selectionOffset);
            else
                transform.localPosition = Vector3.zero;
        }

        public void Deselect()
        {
            if (selected)
            {
                selected = false;
                if (selected)
                    transform.localPosition += (transform.up * 50);
                else
                    transform.localPosition = Vector3.zero;
            }
        }


        public int SiblingAmount()
        {
            return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
        }

        public int ParentIndex()
        {
            return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
        }

        public float NormalizedPosition()
        {
            return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
        }

        
        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    switch (State)
        //    {
        //        case CardStateType.InHand:
        //            transform.localScale *= 1.5f;
        //            transform.position +=new Vector3(0f, 3f, 0f);
        //            break;
        //        case CardStateType.OnTable:
        //            break; 
        //    }
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    switch (State)
        //    {
        //        case CardStateType.InHand:
        //            transform.localScale /= 1.5f;
        //            transform.position -=new Vector3(0f, 3f, 0f);
        //            break;
        //        case CardStateType.OnTable:
        //            break;
        //    }
        //}

        [ContextMenu("Switch Visual")]
        public void SwitchVisual() => _frontCard.SetActive(!IsFrontSide);
    }
}
