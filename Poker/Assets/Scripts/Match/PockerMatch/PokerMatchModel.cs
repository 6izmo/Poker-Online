using Cards;
using System;
using Players;
using Photon.Pun;
using Combination;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using Bank;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;

namespace PokerMatch
{
    public class PokerMatchModel
    {
        public enum MatchPhase
        {
            NewDistribution,
            BetSetting,
            NewDistributionAfterBet,
            EndMatch,
        }

        private Dictionary<Player, PlayerModel> _playerModels = new();
        private Dictionary<CombinationModel, Player> _playerCombination = new();
        private List<Player> _currentPlayers = new();
        private List<CardModel> _tableCards = new();
        private int _countPlayerFolded = 0;
        private int _startPlayerIndex;

        public event Action<CardData> OnNewDistribution;
        public event Action<CardData, int> OnNewDistributionAfterBet;
        public event Action OnBetSettings;
        public event Action OnEndedMatch;
        public event Action<string> OnEndedGame;

        public int SmallBlindPlayerId { get; private set; }

        public Player StartPlayer { get; private set; }

		public CardData CardData { get; private set; }

		public int DesiredCardCount { get; private set; } = 3;

		public PokerPlayerData PokerPlayerData { get; private set; }

        public Player CurrentBetPlayer { get; set; }

		public int PlayersCount => _currentPlayers.Count;

		public List<Player> CurrentPlayers => _currentPlayers;

        public List<CardModel> TableCards => _tableCards;

        public PokerMatchModel(PokerPlayerData data, CardData cardData)    
        {
            PokerPlayerData = data;
            CardData = cardData;
            SmallBlindPlayerId = -1;
        }

        public void AddPlayerCombination(Player player, CombinationModel combination)
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
            _countPlayerFolded++;
            Debug.Log(_countPlayerFolded);
		}

        public Player GetWinner()
        {
            foreach (var item in _playerCombination)
                Debug.Log($"Combination:{item.Key}; Player:{item.Value.NickName}");
              
            List<CombinationModel> combinations = _playerCombination.Keys.ToList();
            combinations.Sort();
			CombinationModel highCombination = combinations.Last();
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
                    TryNewMatch();
					break;
                case MatchPhase.BetSetting:
                    OnBetSettings?.Invoke();
                    break;
                case MatchPhase.NewDistributionAfterBet:       
                    SetStartPlayer();
					CurrentBetPlayer = StartPlayer;    
					Debug.Log($"start player - {CurrentBetPlayer.NickName}");
					OnNewDistributionAfterBet?.Invoke(CardData, DesiredCardCount);
					DesiredCardCount++;
					break;
                case MatchPhase.EndMatch:
                    OnEndedMatch?.Invoke();
                    break;
			}
        }

        private void SetStartPlayer()
        {
			PlayerModel playerModel = GetPlayerModel(StartPlayer);

            if (playerModel.Money.Value != 0 && !playerModel.Folded.Value)
                return;

			for (int i = StartPlayer.ActorNumber; i <= PhotonNetwork.PlayerList.Length; i++)    
            {
                if (i == PhotonNetwork.PlayerList.Length)  
                {  
					i = -1;   
					continue;
				}

				PlayerModel model = _playerModels[PhotonNetwork.PlayerList[i]];
                if (model.Money.Value != 0 && !model.Folded.Value)
                {   
					_startPlayerIndex = i;         
					break; 
				}
			}
            StartPlayer = PhotonNetwork.PlayerList[_startPlayerIndex];   
        }

        private void TryNewMatch()
        {
			DesiredCardCount = 3;
            _countPlayerFolded = 0;
			_tableCards.Clear();  
			_playerCombination.Clear();

			bool response = GetPlayers();
			if (!response)
				return;
			
			SmallBlindPlayerId++;
			if (SmallBlindPlayerId >= PlayersCount)
				SmallBlindPlayerId = 0;

            StartPlayer = _currentPlayers[SmallBlindPlayerId];
            _startPlayerIndex = SmallBlindPlayerId;

			OnNewDistribution?.Invoke(CardData);
		}   

        private bool GetPlayers()
        {
			_currentPlayers = PhotonNetwork.PlayerList.ToList();
            for (int i = 0; i < PlayersCount; i++)
            {
				PlayerModel model = GetPlayerModel(_currentPlayers[i]);
				if (model.Money.Value < BankModel.BigBlind)
                    _currentPlayers.Remove(_currentPlayers[i]);
				else
					model.ResetModel();  
			}
            if (PlayersCount == 1)
            {
                Player winner = _currentPlayers[0];
				OnEndedGame?.Invoke(winner.NickName);
                _playerModels[winner].Folded.Value = false;
                return false;
			}
            return true;
		}
    }
}
