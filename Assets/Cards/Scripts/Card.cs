using JetBrains.Annotations;
using System;
using TMPro;
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
        //private GameObject cloneCard;

        public Transform defaultParent, defaultTempCardParent;
        private GameObject tempCardGO;
        [SerializeField]
        private GameObject cloneStorage;
        private float transparency = 0.5f;

        private void Awake()
        {
            mainCamera = Camera.allCameras[0];
            //CreateClone();

        }
        
        public void Configuration(Material picture, CardPropertiesData data, string description )
        {
            _picture.sharedMaterial = picture;
            _cost.text = data.Cost.ToString();
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _type.text = data.Type == CardUnitType.None ? string.Empty : data.Type.ToString();// если типа нет то пусто, если есть то указывается какой
            _health.text = data.Health.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && State != CardStateType.InDeck) 
            {
                transform.localScale *= 1.5f;
                transform.localPosition += new Vector3(0f, -3f, 0f);

                if (tempCardGO == null)
                {
                    CreateClone();
                }
            }
                
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale /= 1.5f;
            transform.localPosition -= new Vector3(0f, -3f, 0f);
        }


        public void OnDrag(PointerEventData eventData)
        {

            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = transform.position - mainCamera.ScreenToViewportPoint(eventData.position);

            defaultParent = defaultTempCardParent = transform.parent;
            tempCardGO.transform.SetParent(defaultParent);

            transform.SetParent(defaultParent.parent);
            //GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(defaultParent);
            //GetComponent<CanvasGroup>().blocksRaycasts = true;

            tempCardGO.transform.SetParent(cloneStorage.transform);
            tempCardGO.transform.localPosition = cloneStorage.transform.localPosition;
        }

        
        private void CreateClone()
        {
            GameObject clone = Instantiate(gameObject);
            
            clone.transform.localPosition = cloneStorage.transform.localPosition;

            clone.GetComponent<ChangeTransparency>().enabled = true;

            tempCardGO = clone;

        }

       


        [ContextMenu("Switch Visual")]
        public void SwitchVisual() => _frontCard.SetActive(!IsFrontSide);
    }
}
