using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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

        public bool IsFrontSide => _frontCard.activeSelf;

        public CardStateType State { get;  set; }

        private Canvas canvas;
        private Vector3 offset;
        [SerializeField]
        private Camera mainCamera;
        
        public Transform defaultParent, defaultTempCardParent;
        private GameObject tempCardGO;
        private bool isEnlarged = false;

        [SerializeField]
        private bool isDraggable;


        private void Awake()
        {
            mainCamera = Camera.allCameras[0];
            tempCardGO = GameObject.Find("TempCardGO");

        }

        

        public void Configuration(Material picture, CardPropertiesData data, string description )
        {
            _picture.sharedMaterial = picture;
            _cost.text = data.Cost.ToString();
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _type.text = data.Type == CardUnitType.None ? string.Empty : data.Type.ToString();
            _health.text = data.Health.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && State != CardStateType.InDeck 
                && eventData.pointerDrag == null && isEnlarged == false) 
            {
                transform.localScale *= 1.5f;
                transform.localPosition += new Vector3(0f, -3f, 0f);
                isEnlarged = true;
            }
                
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (  State != CardStateType.InDeck && eventData.pointerDrag == null
                && isEnlarged == true)
            {
                transform.localScale /= 1.5f;
                transform.localPosition -= new Vector3(0f, -3f, 0f);
                isEnlarged = false;
            }


        }


        public void OnDrag(PointerEventData eventData)
        {
            if (!isDraggable) return;

            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            if (tempCardGO.transform.parent != defaultTempCardParent)
                tempCardGO.transform.SetParent(defaultTempCardParent);

            CheckPosition();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = transform.position - mainCamera.ScreenToViewportPoint(eventData.position);

            defaultParent = defaultTempCardParent = transform.parent;

            isDraggable = defaultParent.GetComponent<DropPlaceScr>().fieldType == FieldType.SelfHand;
            if (!isDraggable) return;

            tempCardGO.transform.SetParent(defaultParent);
            tempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

            transform.SetParent(defaultParent.parent);

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDraggable) return;

            transform.SetParent(defaultParent);


            transform.SetSiblingIndex(tempCardGO.transform.GetSiblingIndex());
            tempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);            

        }

        
        private void CheckPosition()
        {
            int newIndex = defaultTempCardParent.childCount;

            for (int i = 0; i < defaultTempCardParent.childCount; i++)
            {
                if(transform.position.x < defaultTempCardParent.GetChild(i).position.x)
                {
                    newIndex = i;

                    if (tempCardGO.transform.GetSiblingIndex() < newIndex)
                        newIndex--;

                    break;    
                }
            }

            tempCardGO.transform.SetSiblingIndex((int)newIndex);
        }

        

        [ContextMenu("Switch Visual")]
        public void SwitchVisual() => _frontCard.SetActive(!IsFrontSide);
    }
}
