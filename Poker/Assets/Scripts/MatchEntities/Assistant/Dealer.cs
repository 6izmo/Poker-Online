using Cards;
using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace PokerMatch
{
    public class Dealer
    {
        public static event Action<List<CardModel>> OnDealingEnded;

        private static CardDeck _cardDeck;

		private static int _timeDelay = 750;
        private static int _currentCountCard = 0;

        public static async UniTask StartDealing(CardData cardData, List<Player> players)  
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            _cardDeck = new();
			_currentCountCard = 0;

			for (int j = 0; j < 2; j++)
			{
				for (int i = 0; i < players.Count; i++)
				{
					CardModel newCardModel = _cardDeck.GetRandomCard();
					Player player = players[i];
					object[] content = new object[] { EventCode.AddCard, player, newCardModel, cardData.CardViewPrefab.name, j };
					RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent((byte)EventCode.Dealing, content, eventOptions, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                    await UniTask.Delay(_timeDelay);
                }
			}
            await UniTask.CompletedTask;
        }

        public static async void TableDealing(CardData cardData, int countCard)
        {
			if (!PhotonNetwork.IsMasterClient)
				return;

			List<CardModel> tableModel = new();
            List<CardPresenter> tableCards = new();
            for (int currentCard = _currentCountCard; currentCard < countCard; currentCard++)
            {
                CardModel newCardModel = _cardDeck.GetRandomCard();
                object[] dataPos = new object[] { EventCode.TableCard, cardData.CardDeckPosition, cardData.GetTableCardPosition(currentCard), 360f };
                GameObject cardObject = PhotonNetwork.InstantiateRoomObject(cardData.TableCardPrefab.name, cardData.GetTableCardPosition(currentCard), Quaternion.identity, 0, dataPos);
                CardPresenter cardPresenter = cardObject.GetComponent<CardPresenter>();
                cardPresenter.Init(newCardModel);
                tableCards.Add(cardPresenter);
                tableModel.Add(newCardModel);
                await UniTask.Delay(_timeDelay);
            }
            for (int i = 0; i < tableCards.Count; i++)
                await tableCards[i].Open(true);

            _currentCountCard = countCard;
            OnDealingEnded?.Invoke(tableModel);
        }
    }
}