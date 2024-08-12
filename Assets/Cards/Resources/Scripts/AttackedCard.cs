using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    //private CardManager cardManager;

    //private void Start()
    //{
    //    cardManager= FindFirstObjectByType<CardManager>();
    //}

    public void OnDrop(PointerEventData eventData)
    {

        var card = eventData.pointerDrag.GetComponent<Card>();

        if(card && card.CanHeAttack &&
            transform.parent.GetComponent<DropPlaceScr>().fieldType == FieldType.EnemyField)
        {
            //cardManager.CadsFight(card, GetComponent<Card>());
            //card.ChangeAttackState(false);
        }

    }

    
}
