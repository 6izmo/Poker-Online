using Cards;
using System;
using Players;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;

namespace PokerMatch
{
    public class PokerMatchModel
    {
        public enum MatchPhase
        {
            NewDistribution,
            BetSetting,
            NewDistributionAfterBet,
            OpeningCard,
        }

        private Dictionary<Player, PlayerModel> _playerModels = new();
        private Dictionary<Combination, Player> _playerCombination = new();
        private List<Player> _currentPlayers = new();
        private List<CardModel> _tableCards = new();

        public event Action<CardData> OnNewDistribution;
        public event Action<CardData, int> OnNewDistributionAfterBet;
        public event Action OnBetSettings;
        public event Action OnFinished;

        public int SmallBlindPlayerId { get; private set; }

        public int DesiredCardCount { get; private set; } = 3;

		public int PlayersCount => _currentPlayers.Count;

		public PokerPlayerData PokerPlayerData { get; private set; }

        public CardData CardData { get; private set; }

        public List<Player> CurrentPlayers => _currentPlayers;

        public List<CardModel> TableCards => _tableCards;

        public PokerMatchModel(PokerPlayerData data, CardData cardData)    
        {
            PokerPlayerData = data;
            CardData = cardData;
            SmallBlindPlayerId = -1;
        }

        public void AddPlayerCombination(Player player, Combination combination)
        {
            if (!_playerCombination.ContainsKey(combination))
                _playerCombination.Add(combination, player);
        }

        public void AddPlayerModel(Player player, PlayerModel model)
        {
            if (!_playerModels.ContainsKey(player))
                _playerModels.Add(player, model);
        }

        public void AddCardTable(CardModel cardModel)
        {
            if (!_tableCards.Contains(cardModel))
                _tableCards.Add(cardModel);
        }

        public void RemovePlayer(Player player)
        {
            if(_currentPlayers.Contains(player))
                _currentPlayers.Remove(player);
        }

        public Player GetWinner()
        {
            foreach (var item in _playerCombination)
                Debug.Log($"Combination:{item.Key}; Player:{item.Value.NickName}");
              
            List<Combination> combinations = _playerCombination.Keys.ToList();
            combinations.Sort();
            Combination highCombination = combinations.Last();
            Debug.Log($"High combination:{highCombination};");
            Player player = _playerCombination.GetValueOrDefault(highCombination);
            return player;
        }

        public PlayerModel GetPlayerModel(Player player) => _playerModels.GetValueOrDefault(player);

        public void SetMatchPhase(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.NewDistribution:
                    DesiredCardCount = 3;
                    _tableCards.Clear();
                    _playerCombination.Clear();
                    _currentPlayers = PhotonNetwork.PlayerList.ToList();
                    SmallBlindPlayerId++;
                    if (SmallBlindPlayerId == PlayersCount)
                        SmallBlindPlayerId = 0;
                    OnNewDistribution?.Invoke(CardData);
                    break;
                case MatchPhase.BetSetting:
                    OnBetSettings?.Invoke();
                    break;
                case MatchPhase.NewDistributionAfterBet:
                    OnNewDistributionAfterBet?.Invoke(CardData, DesiredCardCount);
					DesiredCardCount++;
                    break;
                case MatchPhase.OpeningCard:
                    OnFinished?.Invoke();
                    break;
            }
        }
    }
}
