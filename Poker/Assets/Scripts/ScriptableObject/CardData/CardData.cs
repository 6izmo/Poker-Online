using System;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Cards {

    [CreateAssetMenu(fileName = "CardData")]
    public class CardData : ScriptableObject
    {
        [SerializeField] private CardView _cardViewPrefab;
        [SerializeField] private CardView _tableCardPrefab;
        [SerializeField] private Vector2 _cardDeckPosition;
        [SerializeField] private List<CardList> _cardList = new();
        [SerializeField] private List<PlayerCardPositions> _playerCardsPos = new();
        [SerializeField] private List<Vector2> _tablePositions = new();

        public CardView CardViewPrefab => _cardViewPrefab;
        public CardView TableCardPrefab => _tableCardPrefab;
        public Vector2 CardDeckPosition => _cardDeckPosition;

        private static Dictionary<CardSuits, List<Sprite>> _allCardInfo = new();
        private LinkedList<PlayerCardPositions> _positionsList;

        public void Init()
        {
            foreach (CardList cardList in _cardList)
            {
                if(!_allCardInfo.ContainsKey(cardList.CardSuits))
                    _allCardInfo.Add(cardList.CardSuits, cardList.SuitList);
            }

            _positionsList = new(_playerCardsPos);
            SortList(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public static Sprite GetSprite(CardModel card) => _allCardInfo.GetValueOrDefault(card.Suit)[card.Rank];

        public Vector2 GetCardPosition(int playerNumber, int cardNumber) => _positionsList.ElementAtOrDefault(playerNumber).CardPositions[cardNumber];

        public Vector2 GetTableCardPosition(int index) => _tablePositions[index];

        public void SortList(int actorNumber)
        {
            switch (actorNumber)
            {
                case 2:
                    _positionsList.AddFirst(_positionsList.RemoveAndGetLast());
                    break;
                case 3:
                    _positionsList.AddLast(_positionsList.RemoveAndGetFirst());
                    _positionsList.AddLast(_positionsList.RemoveAndGetFirst());
                    break;
                case 4:
                    _positionsList.AddLast(_positionsList.RemoveAndGetFirst());
                    break;
            }
        }
    }

    [Serializable]
    public class CardList
    {
        [SerializeField] private CardSuits _cardSuits;
        [SerializeField] private List<Sprite> _suitList = new(12);

        public CardSuits CardSuits => _cardSuits;
        public List<Sprite> SuitList => _suitList;
    }

    [Serializable]
    public class PlayerCardPositions
    {
        [SerializeField] private List<Vector2> _cardPositions = new();

        public List<Vector2> CardPositions => _cardPositions;
    }
}

