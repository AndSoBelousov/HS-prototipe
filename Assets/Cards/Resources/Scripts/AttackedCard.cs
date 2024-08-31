using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("сработал AttackedCard");
        var card = eventData.pointerDrag.GetComponent<Card>();

        if (card && card.CanHeAttack &&
            transform.parent.GetComponent<DropPlaceScr>().fieldType == FieldType.EnemyField)
        {
            Debug.Log(card + " атаковал " + GetComponent<Card>());
            FindFirstObjectByType<CardManager>().CadsFight(card, GetComponent<Card>());
            card.ChangeAttackState(false);
        }

    }


}
