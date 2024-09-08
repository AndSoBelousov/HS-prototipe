using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {

        [SerializeField]
        private Transform _positions;
        
      
        public bool SetNewCard(Card card)
        {
            if (card == null) return true;
            
            StartCoroutine(MoveInHand(card, _positions) );
            return true;
        }


        private IEnumerator MoveInHand(Card card, Transform target)
        {
            if(target.transform.localPosition.y < 300) // выше находится рука противника, его карты должны быть скрыты
            {
                card.SwitchVisual();
                card.GetComponent<AttackedCard>().enabled = false;
            }
            float time = 0;
            var startPos = card.transform.position;
            var endPos = target.transform.position;
            while (time < 0.6)
            {   
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;

                yield return null;
            }
            FindFirstObjectByType<CardManager>().GetComponent<CardManager>().CheckingTheCost();
            card.GetComponent<Transform>().SetParent(transform);
            card.State = CardStateType.InHand;
           
            
        }

    }
}
