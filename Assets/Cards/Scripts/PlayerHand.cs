using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {
        private Card[] _cardsInHand;

        [SerializeField]
        private Transform[] _positions;

        private void Start ()

        {
            _cardsInHand = new Card[_positions.Length];
        }
        public bool SetNewCard(Card card)
        {
            if (card == null) return true;

            var index = GetLastPosition();

            if (index == -1)
            {
                Destroy(card.gameObject);
                return false;
            }

            _cardsInHand[index] = card;
            StartCoroutine(MoveInHand(card, _positions[index]));

            return true;
        }

        private int GetLastPosition()
        {
            for (int i =0;  i < _cardsInHand.Length; i++)
            {
                if (_cardsInHand[i] == null) return i;
            }
            return -1;
        }

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

            card.State = CardStateType.InHand;
        }


    }
}
