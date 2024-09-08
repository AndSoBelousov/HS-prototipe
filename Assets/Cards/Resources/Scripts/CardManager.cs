using Cards.ScriptableObjects;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VisionOS;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Cards
{
    

    public class CardManager : MonoBehaviour
{
       

        private Material _baseMat;
        public CardPropertiesData[] _allCards;

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

        [SerializeField]
        private int _playerMana, _enemyMana;
        [SerializeField]
        private TextMeshProUGUI _playerManaText, _enemyManaText;
        private int _initialPlaerMana = 3;
        private int _initialEnemyMana = 3;


        public bool IsPlaerTurn
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
            //_baseMat.renderQueue = 2995;

            
            enemyHandList = new List<Card>();
            playerHandList = new List<Card>();

            enemyFieldList = new List<Card>();
            playerFieldList = new List<Card>();
        }

        private void Start()
        {
            playerDeckList = CreateDeck(_deckPlayerParente, playerDeckList);   
            enemyDeckList = CreateDeck(_deckEnemyParente, enemyDeckList);

        }

        private void ManaReplenishment()
        {
            if (IsPlaerTurn)
            {
                _playerMana = _initialPlaerMana;
                if (_initialPlaerMana < 10) _initialPlaerMana++;
            }
            else
            {
                _enemyMana = _initialEnemyMana;
                if(_initialEnemyMana < 10) _initialEnemyMana++;
            }


            UpdatingManaStats();
        }
        private void UpdatingManaStats()
        {
            _playerManaText.text = _playerMana.ToString();
            _enemyManaText.text = _enemyMana.ToString();
        }
        public void StartGame()
        {
            _turn = 0;

            StartCoroutine(IssuingCards(4, _playerHand));
            StartCoroutine(IssuingCards(4, _enemyHand));
            StartCoroutine(TurnFunc());
            ManaReplenishment();
            CheckingTheCost();


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
                int randomNumber = Random.Range(0, _allCards.Length);
                var random = _allCards[randomNumber];
                var picture = new Material(_baseMat);
                picture.mainTexture = random.Texture;
                deck[i].Configuration(picture, random, CardUtility.GetDescriptionById((uint)random.Id), randomNumber);
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
            yield return new WaitForSeconds(0.3f);
            foreach (var card in playerFieldList)
            {
                card.OutlineOnOrOff(false);
            }

            
            if (IsPlaerTurn)
            {
                foreach (var card in playerFieldList)
                {
                    card.ChangeAttackState(true);
                    card.OutlineOnOrOff(card.CanHeAttack);
                }

                while(_turnTime-- > 0) 
                {
                    _turnTimeTxt.text = _turnTime.ToString();
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                foreach (var card in enemyFieldList)
                {
                    card.ChangeAttackState(true);

                }

                while (_turnTime-- > 27)
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
            Debug.Log("Ход противника!");

            //int count = Random.Range(0, enemyCardsInHand.Count);

            for (int i = 0; i < enemyCardsInHand.Count; i++)
            {
                if (_allCards[enemyCardsInHand[i].IdCard].Cost <= _enemyMana)
                {
                    _enemyMana -= _allCards[enemyCardsInHand[i].IdCard].Cost;
                    enemyCardsInHand[i].transform.SetParent(_enemyFieldTranform);
                    enemyCardsInHand[i].SwitchVisual();

                    OnCardMovedToField(enemyCardsInHand[i], false);
                }
            }

            foreach (var activeCard in enemyFieldList.FindAll(x => x.CanHeAttack))
            {
                Debug.Log("попытка атаки ");
                if (playerFieldList.Count == 0)
                {
                    Debug.Log("у противника нет карт ");
                    return;
                }

                var enemy = playerFieldList[Random.Range(0, playerFieldList.Count)];

                activeCard.ChangeAttackState(false);
                
                CadsFight(enemy, activeCard);
            }

        }


        public void ChangeTurn()
        {
            StopAllCoroutines ();
            _turn++;

            _endTimeBtn.interactable = IsPlaerTurn;
            CheckingTheCost();
            ManaReplenishment();
            PlayerHand player = IsPlaerTurn ? _playerHand : _enemyHand;
            
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

        public void CadsFight(Card playerCard, Card enemyCard)
        {
            CardPropertiesData playerData = _allCards[playerCard.IdCard];
            CardPropertiesData enemyData = _allCards[enemyCard.IdCard];
            Debug.Log("Запущен метод CardsFight, здоровье игроков" + playerData.Health + " и " + enemyData.Health);

            playerData.Health -= enemyData.Attack;
            enemyData.Health -= playerData.Attack;

            playerCard.RefreshData(playerData);
            enemyCard.RefreshData(enemyData);

            Debug.Log("атакующая карта " + playerData.Name + "Здоровье атакующей карты - " + playerData.Health);
            Debug.Log("атакованная карта " + enemyData.Name + "Здоровье атакованной карты - " + enemyData.Health);
            DestroyCard(playerCard, playerData);
            DestroyCard(enemyCard,enemyData);
        }

        public void HerosAttack(Card card, HeroIIcon hero)
        {
            CardPropertiesData cardData = _allCards[card.IdCard];

            hero.Health -= cardData.Attack;
            hero.RefreshHealth();

            if (hero.Health <= 0)
            {
                Debug.Log(" Игра окончена!");

            }
        }

        private void DestroyCard(Card card, CardPropertiesData cardData)
        {
            Debug.Log("DestroyCard сработал ");

            card.OnEndDrag(null);
            //CardPropertiesData cardPropertiesData = _allCards[card.IdCard];

            if (cardData.Health <= 0)
            {
                Debug.Log("карта мертва по всем показателям");

                if (enemyFieldList.Exists(x => x == card))
                    enemyFieldList.Remove(card);

                if(playerFieldList.Exists(x => x == card))
                    playerFieldList.Remove(card);

                Destroy(card.gameObject);
            }
        }

        public void CheckingTheCost()
        {

            for(int i = 0; i < playerHandList.Count; i++)
            {
                
                if (playerHandList[i] != null && _allCards[playerHandList[i].IdCard].Cost <= _playerMana)
                {
                    playerHandList[i].ChangeCosteColor(Color.green);
                    Debug.Log(_allCards[playerHandList[i].IdCard].Cost);
                }
                else
                {
                    playerHandList[i].ChangeCosteColor(Color.white);
                }
            }
           
        }
        
        public void PaymentForACard(Card card)
        {
            _playerMana -= _allCards[card.IdCard].Cost;
            UpdatingManaStats();
            CheckingTheCost();
        }

    }
}

