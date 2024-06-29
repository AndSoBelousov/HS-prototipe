using Cards.ScriptableObjects;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Cards
{
    

    public class CardManager : MonoBehaviour
{
       

        private Material _baseMat;
        private CardPropertiesData[] _allCards;

        [SerializeField]
        public List<Card> enemyDeckList, playerDeckList,
                          enemyHandList, playerHandList,
                          enemyFieldList, playerFieldList;

        [SerializeField, UnityEngine.Range(5f, 100f)]
        private int _cardInDeck = 30;
        [SerializeField]
        private Card _cardPrefab;
        [SerializeField]
        private CardPackConfiguration[] _packs;

        [Space, SerializeField]
        private Transform _deckPlayerParente;
        [SerializeField]
        private Transform _deckEnemyParente;
        [SerializeField]
        private PlayerHand _playerHand;
        [SerializeField]
        private PlayerHand _enemyHand;
        [SerializeField]
        private Transform _playerFieldTransform;
        [SerializeField]
        private Transform _enemyFieldTranform;

        private int _turn, _turnTime = 60;
        [SerializeField]
        private UnityEngine.UI.Button _endTimeBtn;
        [SerializeField]
        private TextMeshProUGUI _turnTimeTxt;

        public bool IsPlaterTurn
        {
            get
            {
                return _turn % 2 == 0;
            }
        }


        private void Awake()
        {
            IEnumerable<CardPropertiesData> cards = new List<CardPropertiesData>();

            foreach (CardPackConfiguration pack in _packs) cards = pack.UnionProperties(cards);
            _allCards = cards.ToArray();

            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;

            
            enemyHandList = new List<Card>();
            playerHandList = new List<Card>();

            enemyFieldList = new List<Card>();
            playerFieldList = new List<Card>();
        }

        private void Start()
        {
            playerDeckList = CreateDeck(_deckPlayerParente, playerDeckList);   
            enemyDeckList = CreateDeck(_deckEnemyParente, enemyDeckList);

            _turn = 0;

            StartCoroutine(IssuingCards(4, _playerHand));
            StartCoroutine(IssuingCards(4, _enemyHand));
            StartCoroutine(TurnFunc());
        }

        private List<Card> CreateDeck(Transform parent, List<Card> listCards)
        {
            var deck = new List<Card>();
            var offset = new Vector3(0f,0f, 0f);
            for(int i =0; i < _cardInDeck; i++) 
            {
                deck.Add(Instantiate(_cardPrefab, parent));
                if (deck[i].IsFrontSide) deck[i].SwitchVisual(); 
                deck[i].transform.transform.localPosition = offset;
                offset.y += 0.5f;

                var random = _allCards[Random.Range(0, _allCards.Length)];
                var picture = new Material(_baseMat);
                picture.mainTexture = random.Texture;
                deck[i].Configuration(picture, random, CardUtility.GetDescriptionById(random.Id));
                listCards.Add(deck[i]);
            }

            return deck;

        }

        IEnumerator IssuingCards(int numberOfCards, PlayerHand player )
        {
            yield return new WaitForSeconds(0.2f);
            List<Card> playerCardsDeck = player == _playerHand ? playerDeckList : enemyDeckList;
            List<Card> cardsInHand = player == _playerHand ? playerHandList : enemyHandList;

            for (int i = 0; i < numberOfCards; i++)
            {
                Card index = null;
                for (int j = playerCardsDeck.Count - 1; j >= 0; j--)
                {
                    if (playerCardsDeck[j] != null)
                    {
                        index = playerCardsDeck[j];
                        playerCardsDeck[j] = null;
                        break;
                    }
                }
                cardsInHand.Add(index);
                player.SetNewCard(index);
                
            }
        }

        IEnumerator TurnFunc()
        {
            _turnTime = 30;
            _turnTimeTxt.text = _turnTime.ToString();
            yield return new WaitForSeconds(0.5f);

            if(IsPlaterTurn)
            {
                while(_turnTime-- > 0) 
                {
                    _turnTimeTxt.text = _turnTime.ToString();
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                while (_turnTime-- > 25)
                {
                    _turnTimeTxt.text = _turnTime.ToString();
                    yield return new WaitForSeconds(1);
                }

                if (enemyHandList.Count > 0)
                {
                    EnemyTurn(enemyHandList);
                }
            }

            ChangeTurn();
        }

         private void EnemyTurn(List<Card> enemyCardsInHand)
        {
            int count = Random.Range(0, enemyCardsInHand.Count);

            for (int i = 0; i < count; i++)
            {
                enemyCardsInHand[0].transform.SetParent(_enemyFieldTranform);
                enemyCardsInHand[0].SwitchVisual();

                OnCardMovedToField(enemyCardsInHand[0], false);
            }
        }

        public void ChangeTurn()
        {
            StopAllCoroutines ();
            _turn++;

            _endTimeBtn.interactable = IsPlaterTurn;

            PlayerHand player = IsPlaterTurn ? _playerHand : _enemyHand;
            StartCoroutine( IssuingCards(1, player));
            StartCoroutine(TurnFunc());

        }

        //методы для обновления списков карт и управления перемещениями:
        public void MoveCard(Card card, List<Card> sourceList, List<Card> targetList)
        {
            if (sourceList.Contains(card))
            {
                sourceList.Remove(card);
                targetList.Add(card);
            }
            else
            {
                Debug.LogWarning("Card not found in the source list!");
            }
        }

        public void OnCardMovedToField(Card card, bool isPlayer)
        {
            if (isPlayer)
            {
                MoveCard(card, playerHandList, playerFieldList);
            }
            else
            {
                MoveCard(card, enemyHandList, enemyFieldList);
            }
        }   

        public void InformationAboutTheLista()
        {
            for (int i = 0; i < playerHandList.Count; i++)
            {
                Debug.Log(playerHandList[i]);
            }
            for (int i = 0;i < playerFieldList.Count;i++)
            {
                Debug.Log(playerFieldList[i]);
            }
        }


    }
}

