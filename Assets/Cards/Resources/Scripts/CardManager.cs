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
        //public Game _currentGame;
        [SerializeField]
        public List<Card> enemyDeckList, playerDeckList,
                          enemyHandList, playerHandList,
                          enemyFieldList, playerFieldList;

        //private Card[] _playerOneCards;
        //private Card[] _playerTwoCards;

        [SerializeField, UnityEngine.Range(5f, 100f)]
        private int _cardInDeck = 30;
        [SerializeField]
        private Card _cardPrefab;
        [SerializeField]
        private CardPackConfiguration[] _packs;

        [Space, SerializeField]
        private Transform _deckOneParente;
        [SerializeField]
        private Transform _deckTwoParente;
        [SerializeField]
        private PlayerHand _player1;
        [SerializeField]
        private PlayerHand _player2;

        //[SerializeField]
        //private int _turnTimeSettings = 60;
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
            playerDeckList = CreateDeck(_deckOneParente, playerDeckList);   
            enemyDeckList = CreateDeck(_deckTwoParente, enemyDeckList);

            _turn = 0;

            StartCoroutine(IssuingCards(4, _player1));
            StartCoroutine(IssuingCards(4, _player2));
            StartCoroutine(TurnFunc());
        }

        private List<Card> CreateDeck(Transform parent, List<Card> listCards)
        {
            var deck = new List<Card>();
            var offset = new Vector3(0f,0f, 0f);
            for(int i =0; i < _cardInDeck; i++) 
            {
                if (_cardPrefab == null) Debug.Log("Нет сраного префаба и где блять он?");
                if (parent == null) Debug.Log("нет сраного родителя который блять на прямую указан как блять как? ");
                if (deck == null) Debug.Log("ну тут я вообще хз, с какго хуя не этого дек, дек блять есть, посмотри выше дура");
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
            List<Card> playerCardsDeck = player == _player1 ? playerDeckList : enemyDeckList;
            List<Card> cardsInHand = player == _player1 ? playerHandList : enemyHandList;

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
                while (_turnTime-- > 23)
                {
                    _turnTimeTxt.text = _turnTime.ToString();
                    yield return new WaitForSeconds(1);
                }
            }

            ChangeTurn();
        }
        public void ChangeTurn()
        {
            StopAllCoroutines ();
            _turn++;

            _endTimeBtn.interactable = IsPlaterTurn;

            PlayerHand player = IsPlaterTurn ? _player1 : _player2;
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

