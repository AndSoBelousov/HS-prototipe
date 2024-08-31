using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedHero : MonoBehaviour, IDropHandler
{

    private HeroIIcon _heroIcon;

    private void Start()
    {
        _heroIcon = GetComponent<HeroIIcon>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("сработал AttackedHero");
        var card = eventData.pointerDrag.GetComponent<Card>();

        if (card && card.CanHeAttack)
        {
            Debug.Log(card + " атаковал героя" );
            FindFirstObjectByType<CardManager>().HerosAttack(card, _heroIcon);

        }
        // включить этот скрипт в HeroIcon, добавить жизни героев в CardManager и там же создать класс на подобе CadsFight, который будет отбавлять жизни у героев 
    }
}
