using Cards.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cards
{
public class CardManager : MonoBehaviour
{
        private Material _baseMat;
        private CardPropertiesData[] _allCards;

        private Card[] _player1Cards;
        private Card[] _player2Cards;

        [SerializeField, Range(5f, 100f)]
        private int _cardInDeck = 30;
        [SerializeField]
        private Card _cardPrefab;
        [SerializeField]
        private CardPackConfiguration[] _packs;

        [Space, SerializeField]
        private Transform _deck1Parente;
        [SerializeField]
        private Transform _deck2Parente;
        [SerializeField]
        private PlayerHand _player1;
        [SerializeField]
        private PlayerHand _player2;


        private void Awake()
        {
            IEnumerable<CardPropertiesData> cards = new List<CardPropertiesData>();

            foreach (CardPackConfiguration pack in _packs) cards = pack.UnionProperties(cards);
            _allCards = cards.ToArray();

            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
        }

        private void Start()
        {
            _player1Cards = CreateDeck(_deck1Parente);
            //_player2Cards = CreateDeck(_deck2Parente);
            //StartCoroutine(IssuingCards());
        }

        private Card[] CreateDeck(Transform parent)
        {
            var deck = new Card[_cardInDeck];
            var offset = new Vector3(0f,0f, 0f);
            for(int i =0; i < _cardInDeck; i++) 
            {
                deck[i] = Instantiate(_cardPrefab, parent);
                if (deck[i].IsFrontSide) deck[i].SwitchVisual(); 
                deck[i].transform.transform.localPosition = offset;
                offset.y += 0.5f;

                var random = _allCards[Random.Range(0, _allCards.Length)];
                var picture = new Material(_baseMat);
                picture.mainTexture = random.Texture;
                deck[i].Configuration(picture, random, CardUtility.GetDescriptionById(random.Id));
            }

            return deck;

        }
        
        IEnumerator IssuingCards()
        {
           yield return new WaitForSeconds(1);

            int numberOfCards = 7;
            for(int i = 0; i < numberOfCards; i++)
            {
                Card index = null;
                for (int j = _player1Cards.Length - 1; j >= 0; j--)
                {
                    if (_player1Cards[j] != null)
                    {
                        index = _player1Cards[j];
                        _player1Cards[j] = null;
                        break;
                    }
                }
                _player1.SetNewCard(index);
                yield return new WaitForSeconds(0.3f);
            }

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Card index = null;
                for (int i = _player1Cards.Length - 1; i >= 0; i--)
                {
                    if (_player1Cards[i] != null)
                    {
                        index = _player1Cards[i];
                        _player1Cards[i] = null;
                        break;
                    }
                }
                _player1.SetNewCard(index);
            }
        }
    }
}

