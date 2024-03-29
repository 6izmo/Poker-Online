using Cards;
using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PokerMatch
{
    public class Dealer 
    {
        public event Action<List<CardModel>> OnDealingEnded;

        private CardDeck _cardDeck;

		private int _timeDelay = 500;
        private int _currentCountCard = 0;

        public async void StartDealing(CardData cardData)
        {
            _cardDeck = new();
            _currentCountCard = 0;

            if (!PhotonNetwork.IsMasterClient)
                return;

			for (int cardIndex = 0; cardIndex < 2; cardIndex++)
			{
				for (int playerIndex = 0; playerIndex < PhotonNetwork.PlayerList.Length; playerIndex++)
				{
					CardModel newCardModel = _cardDeck.GetRandomCard();
					Player player = PhotonNetwork.PlayerList[playerIndex];
					object[] content = new object[] { EventCode.AddCard, player, newCardModel, cardData.CardViewPrefab.name, cardIndex };
					RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent((byte)EventCode.Dealing, content, eventOptions, ExitGames.Client.Photon.SendOptions.SendUnreliable);
					await Task.Delay(_timeDelay);
                }
			}
		}

        public async void TableDealing(CardData cardData, int countCard)
        {
			List<CardModel> tableModel = new();
			if (PhotonNetwork.IsMasterClient)
            {
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
					await Task.Delay(_timeDelay);
				}
				for (int i = 0; i < tableCards.Count; i++)
                    await tableCards[i].Open(true);

                _currentCountCard = countCard;
			}            
            OnDealingEnded?.Invoke(tableModel);
        }
    }
}
