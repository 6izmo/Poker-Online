using Bank;
using Cards;
using System;
using Players;
using Photon.Pun;
using Combination;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;  

namespace PokerMatch  
{
    public class MatchModel
    {
        public enum MatchPhase
        {
            NewDistribution,
            BetSetting,
            NewDistributionAfterBet,
            EndMatch,
        }

        private Dictionary<Player, PlayerModel> _playerModels = new();
        private Dictionary<Player, PlayerInfoView> _playersInfo = new();
        private Dictionary<Player, PlayerItemPosition> _playerItemPosition = new();
        private Dictionary<CombinationModel, Player> _playerCombination = new();
        private List<Player> _currentPlayers = new();
        private List<CardModel> _tableCards = new();
        private int _startPlayerIndex;

        public event Action<CardData> OnNewDistribution;
        public event Action<CardData, int> OnNewDistributionAfterBet;
        public event Action OnBetSettings;
        public event Action OnEndedMatch;
        public event Action<string> OnEndedGame;

        public int SmallBlindId { get; private set; }

        public Player StartPlayer { get; private set; }

        public Player BigBlindPlayer { get; private set; }

        public Player CurrentBetPlayer { get; set; }

        public CardData CardData { get; private set; }

        public PokerPlayerData PokerPlayerData { get; private set; }  

        public int DesiredCardCount { get; private set; } = 3;

        public int PlayersCount => _currentPlayers.Count;

        public List<Player> CurrentPlayers => _currentPlayers;

        public List<CardModel> TableCards => _tableCards;

        public MatchModel(PokerPlayerData data, CardData cardData, Dictionary<Player, PlayerInfoView> playersInfo)
        {
            _playersInfo = playersInfo;
            PokerPlayerData = data;
            CardData = cardData;

            SmallBlindId = -1;
        }

        public void AddPlayerModel(Player player, PlayerModel model)
        {
            if (!_playerModels.ContainsKey(player))
                _playerModels.Add(player, model);
        }

		public void AddCombination(Player player, CombinationModel combination)
		{
			if (!_playerCombination.ContainsKey(combination))
				_playerCombination.Add(combination, player);
		}

        public Vector2 GetItemPosition(Player player)
        {
            if (player == PhotonNetwork.LocalPlayer)
                return PokerPlayerData.LocalPosition.BlindPosition;
            return _playerItemPosition.GetValueOrDefault(player).BlindPosition;
        }

		public PlayerInfoView GetPlayerInfo(Player player) => _playersInfo.GetValueOrDefault(player);

        public void AddCardTable(CardModel cardModel)  
        {
            if (!_tableCards.Contains(cardModel))
                _tableCards.Add(cardModel);
        }

        public void AddPlayerItemPosition(Player player, PlayerItemPosition position)
        {
            if (!_playerItemPosition.ContainsKey(player))
                _playerItemPosition.Add(player, position);
        }

        public void RemovePlayer(Player player)
        {
            if (!_currentPlayers.Contains(player))
                return;
            PhotonNetwork.DestroyPlayerObjects(player);
            _currentPlayers.Remove(player); 
        }

        public Player GetWinner() 
        {
            foreach (var item in _playerCombination)
                Debug.Log($"Combination:{item.Key}; Player:{item.Value.NickName}");

            List<CombinationModel> combinations = _playerCombination.Keys.ToList();
            combinations.Sort();
            CombinationModel highCombination = combinations.Last();
            Debug.Log($"High combination:{highCombination};");      
            return _playerCombination.GetValueOrDefault(highCombination);
		}

        public PlayerModel GetPlayerModel(Player player) => _playerModels.GetValueOrDefault(player);

        public void SetMatchPhase(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.NewDistribution:
                    ClearData();
                    SetStartData();
                    break;
                case MatchPhase.BetSetting:
                    OnBetSettings?.Invoke();
                    break;
                case MatchPhase.NewDistributionAfterBet:
                    OnNewDistributionAfterBet?.Invoke(CardData, DesiredCardCount);
                    DesiredCardCount++;
                    break;
                case MatchPhase.EndMatch:
                    OnEndedMatch?.Invoke();
                    break;
            }
        }

        public void SetStartPlayer()
        {
            PlayerModel playerModel = _playerModels[StartPlayer];
            if (playerModel.Money.Value == 0 || playerModel.Folded.Value)
            {
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
            CurrentBetPlayer = StartPlayer;
        }

        private void ClearData()
        {
			DesiredCardCount = 3;
			_tableCards.Clear();
			_playerCombination.Clear();
			ClearPlayers();
        }

        private void SetStartData()
        {
            if (PlayersCount == 1)
            {
                Player winner = _currentPlayers[0];
                _playerModels[winner].Folded.Value = false;
                OnEndedGame?.Invoke(winner.NickName);
                return;
            }
            SetPlayerBlind();
        }

        private void SetPlayerBlind()
        {
            SmallBlindId = SmallBlindId + 1 >= PlayersCount ? 0 : SmallBlindId + 1;
            _startPlayerIndex = SmallBlindId;
            StartPlayer = _currentPlayers[_startPlayerIndex];
            BigBlindPlayer = _startPlayerIndex == PlayersCount - 1 ? _currentPlayers[0] : _currentPlayers[_startPlayerIndex + 1];

            OnNewDistribution?.Invoke(CardData);
        }

        private void ClearPlayers()
        {
            _currentPlayers = PhotonNetwork.PlayerList.ToList();
            for (int i = 0; i < PlayersCount; i++)
            {
                PlayerModel model = GetPlayerModel(_currentPlayers[i]);
                if (model.Money.Value < BankModel.BigBlind)
                {
                    model.Folded.Value = true;
                    RemovePlayer(_currentPlayers[i]);
                    i--;
                }
                else
                    model.ResetModel();
            }      
        }
    }
}
