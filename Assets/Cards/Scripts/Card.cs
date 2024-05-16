using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
            switch (State)
            {
                case CardStateType.InHand:
                    transform.localScale *= 1.5f;
                    transform.position +=new Vector3(0f, 3f, 0f);
                    break;
                case CardStateType.OnTable:
                    break; 
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InHand:
                    transform.localScale /= 1.5f;
                    transform.position -=new Vector3(0f, 3f, 0f);
                    break;
                case CardStateType.OnTable:
                    break;
            }
        }

        [ContextMenu("Switch Visual")]
        public void SwitchVisual() => _frontCard.SetActive(!IsFrontSide);
    }
}
