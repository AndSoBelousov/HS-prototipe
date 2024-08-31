using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroIIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ChoosingHero1 _choos;

    private int _health = 30;
    [SerializeField]
    private GameObject _healthUI;
    [SerializeField]
    private TextMeshProUGUI _healthText;

    public int Health
        { get { return _health; } set { _health = value; } }

    public void RefreshHealth()
    {
        _healthText.text = _health.ToString();
    }
    private void Start()
    {
        _choos = FindFirstObjectByType<ChoosingHero1>();     
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
            transform.localScale *= 1.5f;
            transform.localPosition += new Vector3(0f, -3f, 0f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale /= 1.5f;
        transform.localPosition -= new Vector3(0f, -3f, 0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerClick != null)
        {
            
            _choos.MovingIcon(eventData.pointerClick.gameObject);

            _healthUI.SetActive(true);
            this.enabled = false; 

        }

    }
}
