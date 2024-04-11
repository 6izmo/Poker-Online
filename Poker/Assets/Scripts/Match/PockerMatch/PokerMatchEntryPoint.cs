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
    public class PokerMatchEntryPoint : MonoBehaviour
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

        private PokerMatchPresenter _pokerPresenter;
        private PokerMatchView _pokerMatchView;

        private MoveHandler _moveHandler;
        private CardSpawner _cardSpawner;

        private void Awake()
        {
            _pokerPresenter = GetComponent<PokerMatchPresenter>();
            _pokerMatchView = GetComponent<PokerMatchView>();
            PlayerListInit();
        }

        private void PlayerListInit()
        {
            PlayerListModel playerListModel = new(_playerListView, _playerData.PlayerItemPrefab);
            _playerListPresenter.Init(playerListModel, _playerListView);
            _playerListView.Init(_playerListPresenter);

            _playerListPresenter.OnAllPlayersReady += StartPokerMatch;
        }

        public void StartPokerMatch(Dictionary<Player, PlayerInfoView> playersInfo)
        {
            PlayerModel playerModel = new(_pokerConfig.StartPlayerMoney);
			new PlayerPresenter(playerModel, _playerView);
            _playerView.Init(playerModel);
            PokerMatchInit();
            _pokerPresenter.StartMatch(playerModel, playersInfo);   
        }

        private void PokerMatchInit()
        {
            _playerData.Init(PhotonNetwork.LocalPlayer.ActorNumber);
            _cardData.Init();

            BankModel bankModel = new(_pokerConfig.SmallBlind, _pokerConfig.BigBlind);
			BankPresenter bankPresenter = new(bankModel, _bankView);

            PokerMatchModel holdem = new(_playerData, _cardData);
			_cardSpawner = new(holdem);
            Dealer dealer = new Dealer();

            _pokerPresenter.Init(holdem, _pokerMatchView, bankPresenter, dealer);
			_moveHandler = new(_pokerPresenter, holdem);

			RegisterType();
		}

        private void RegisterType()
        {
			PhotonPeer.RegisterType(typeof(CardModel), 4, CardModel.Serialize, CardModel.Deserialize);
			PhotonPeer.RegisterType(typeof(PlayerModel), 5, PlayerModel.Serialize, PlayerModel.Deserialize);
			PhotonPeer.RegisterType(typeof(CombinationModel), 6, CombinationModel.Serialize, CombinationModel.Deserialize);
			PhotonPeer.RegisterType(typeof(ColorModel), 7, ColorModel.Serialize, ColorModel.Deserialize);
		}

		private void OnDestroy()
		{
			_moveHandler.RemoveCallback();
			_cardSpawner.RemoveCallback();
		}
    }
}
