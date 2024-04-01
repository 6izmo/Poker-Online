using Bank;
using Cards;
using Players;
using Photon.Pun;
using PlayerList;
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

        [Space]
        [SerializeField] private BankView _bankView;

        [Space]
        [SerializeField] private PlayerView _playerView;

        private PokerMatchPresenter _pokerPresenter;
        private PokerMatchView _pokerMatchView;

        private void Awake()
        {
            _pokerPresenter = GetComponent<PokerMatchPresenter>();
            _pokerMatchView = GetComponent<PokerMatchView>();
            PlayerListInit();
        }

        private void PlayerListInit()
        {
            PlayerListModel playerListModel = new(_playerListView, _playerData.PlayerItemPrefab);
            _playerListPresenter.Init(playerListModel);
            _playerListView.Init(_playerListPresenter);

            _playerListPresenter.OnAllPlayersReady += StartPokerMatch;
        }

        public void StartPokerMatch(Dictionary<Player, PlayerInfoView> playersInfo)
        {
            PlayerModel playerModel = new(_pokerConfig.StartPlayerMoney);
			PlayerPresenter playerPresenter = new(playerModel, _playerView);
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
            CardSpawner cardSpawner = new(holdem);
            Dealer dealer = new();

            _pokerPresenter.Init(holdem, _pokerMatchView, bankPresenter, dealer);
            MoveHandler moveHandler = new(_pokerPresenter, holdem); 

            PhotonPeer.RegisterType(typeof(CardModel), 4, CardModel.Serialize, CardModel.Deserialize);
            PhotonPeer.RegisterType(typeof(PlayerModel), 5, PlayerModel.Serialize, PlayerModel.Deserialize);
            PhotonPeer.RegisterType(typeof(Combination), 6, Combination.Serialize, Combination.Deserialize);
            PhotonPeer.RegisterType(typeof(ColorModel), 7, ColorModel.Serialize, ColorModel.Deserialize);
        }

        private void OnDisable() => _playerListPresenter.OnAllPlayersReady -= StartPokerMatch;
    }
}
