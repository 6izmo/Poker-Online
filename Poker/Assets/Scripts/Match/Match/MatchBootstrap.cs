using Bank;
using Cards;
using Players;
using Photon.Pun;
using PlayerList;
using Combination;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace PokerMatch
{
    public class MatchBootstrap : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PokerPlayerData _playerData;
        [Space]
        [SerializeField] private CardData _cardData;
        [Space]
        [SerializeField] private PokerSettings _pokerConfig;  

        [Header("PlayerList")]
        [SerializeField] private PlayerListPresenter _playerListPresenter;
        [SerializeField] private PlayerListView _playerListView;

        [Header("Views")]
        [SerializeField] private BankView _bankView;
        [Space]
        [SerializeField] private PlayerView _playerView;

        [SerializeField] private MatchService _matchService;
        [Space]
        [SerializeField] private MatchView _pokerMatchView;

        private PlayerModel _playerModel;

        private void Awake()
        {
			PlayerListModel playerListModel = new(_playerListView, _playerData.PlayerItemPrefab);
			_playerListPresenter.Init(playerListModel, _playerListView);
			_playerListView.Init(_playerListPresenter);

			_playerListPresenter.OnAllPlayersReady += MatchInitialize;
		}

        private void MatchInitialize(Dictionary<Player, PlayerInfoView> playersInfo)
        {
			DataInitialize();
			PlayerInitialize();

			BankModel bankModel = new(_pokerConfig.SmallBlind, _pokerConfig.BigBlind);
            BankPresenter bankPresenter = new(bankModel, _bankView); 

            MatchModel holdem = new(_playerData, _cardData, playersInfo);
			new CardSpawner(holdem);

			_matchService.Init(holdem, bankPresenter);
			_matchService.AddPlayerModel(_playerModel);
		    new MoveHandler(_matchService, holdem);

            MatchPresenter matchPresenter = new (holdem, _pokerMatchView, _matchService);
            matchPresenter.StartMatch();
		}

        private void PlayerInitialize()
        {
			_playerModel = new(_pokerConfig.StartPlayerMoney);
            new PlayerPresenter(_playerModel, _playerView);
			_playerView.Init(_playerModel);
		}

        private void DataInitialize()
        {
			_playerData.Init(PhotonNetwork.LocalPlayer.ActorNumber);
			_cardData.Init();
			RegisterType();
		}

        private void RegisterType()
        {
			PhotonPeer.RegisterType(typeof(CardModel), 4, CardModel.Serialize, CardModel.Deserialize);
			PhotonPeer.RegisterType(typeof(PlayerModel), 5, PlayerModel.Serialize, PlayerModel.Deserialize); 
			PhotonPeer.RegisterType(typeof(CombinationModel), 6, CombinationModel.Serialize, CombinationModel.Deserialize);
			PhotonPeer.RegisterType(typeof(ColorModel), 7, ColorModel.Serialize, ColorModel.Deserialize);
		}

		private void OnDestroy() => EventUnsubscriber.Execute();
    }
}
