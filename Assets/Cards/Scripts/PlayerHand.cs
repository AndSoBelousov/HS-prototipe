using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {
        private Card[] _cardsInHand;

        [SerializeField]
        private Transform _positions;
        
      
        public bool SetNewCard(Card card)
        {
            if (card == null) return true;
            
            StartCoroutine(MoveInHand(card, _positions) );
            return true;
        }

        //private int GetLastPosition()
        //{
        //    for (int i =0;  i < _cardsInHand.Length; i++)
        //    {
        //        if (_cardsInHand[i] == null) return i;
        //    }
        //    return -1;
        //}

        private IEnumerator MoveInHand(Card card, Transform target)
        {
            card.SwitchVisual();
            float time = 0;
            var startPos = card.transform.position;
            var endPos = target.transform.position;
            while (time < 1)
            {   
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;

                yield return null;
            }

            card.GetComponent<Transform>().SetParent(transform);
            card.State = CardStateType.InHand;
        }


    }
}
