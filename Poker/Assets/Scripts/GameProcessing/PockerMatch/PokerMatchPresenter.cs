using Bank;
using Cards;
using Players;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;
using Phase = PokerMatch.PokerMatchModel.MatchPhase;

namespace PokerMatch
{
    [RequireComponent(typeof(PhotonView))]
    public class PokerMatchPresenter : MonoBehaviourPunCallbacks
    {
        private Dealer _dealer;
        private PokerMatchView _matchView;
        private PokerMatchModel _matchModel;
        private BankPresenter _bankPresenter;
        private Dictionary<Player, PlayerInfoView> _playersInfo;

        private const int _delayOperation = 1000;

        public void Init(PokerMatchModel model, PokerMatchView view, BankPresenter bankPreseter, Dealer dealer)
        {
            _dealer = dealer;
            _matchView = view;
            _matchModel = model;
            _bankPresenter = bankPreseter;
                 
            _matchModel.OnNewDistribution += (content) => _matchView.CardDeckActivate();
            _matchModel.OnNewDistribution += (context) => StartNewBidding();
            _matchModel.OnNewDistribution += _dealer.StartDealing;

            _matchModel.OnNewDistributionAfterBet += _dealer.TableDealing;
            _matchModel.OnBetSettings += StartBidding;
            _matchModel.OnFinished += FinishGame;

            _dealer.OnDealingEnded += EndDealing;
        }

        public async void StartMatch(PlayerModel model, Dictionary<Player, PlayerInfoView> playersInfo)
        {
            photonView.RPC("AddPlayerModel", RpcTarget.All, PhotonNetwork.LocalPlayer, model);
            _bankPresenter.ActivateBank();
            _playersInfo = playersInfo;
            SetPlayerInPlaces();
            await Task.Delay(_delayOperation);
            _matchModel.SetMatchPhase(Phase.NewDistribution);  
        }

        private void SetPlayerInPlaces()
        {
            _matchView.ShowPlayersInPlaces(_playersInfo.GetValueOrDefault(PhotonNetwork.LocalPlayer), _matchModel.PokerPlayerData.LocalPosition);
			for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            {
                PlayerInfoView playerItem = _playersInfo.GetValueOrDefault(PhotonNetwork.PlayerListOthers[i]);
                PlayerItemPosition position = _matchModel.PokerPlayerData.GetPosition(i);
                _matchView.ShowPlayersInPlaces(playerItem, position);
            }
        }

        private void EndDealing(List<CardModel> models)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < models.Count; i++)
                    photonView.RPC("AddTableCard", RpcTarget.All, models[i]);
            }
            _matchModel.SetMatchPhase(Phase.BetSetting);
		}

        private void StartBidding()
        {
            for (int i = 0; i < _matchModel.CurrentPlayers.Count; i++)
            {
                PlayerModel playerModel = _matchModel.GetPlayerModel(_matchModel.CurrentPlayers[i]);
                playerModel.Rate.Value = 0;
            }
            _bankPresenter.ChangeRate(0);
            if (PhotonNetwork.IsMasterClient)
                SetPlayerStatePun(_matchModel.CurrentPlayers[_matchModel.SmallBlindPlayerId], PlayerModel.PlayerState.Move);   
        }

        private void StartNewBidding()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            SetPlayerStatePun(PhotonNetwork.PlayerList[_matchModel.SmallBlindPlayerId], PlayerModel.PlayerState.Move);
            SetBlinds(PhotonNetwork.PlayerList[_matchModel.SmallBlindPlayerId]);
        }

        private void SetBlinds(Player player)
        {
            Player bigBlindPlayer = player.ActorNumber == PhotonNetwork.PlayerList.Length ? PhotonNetwork.PlayerList[0] : PhotonNetwork.PlayerList[player.ActorNumber];
            UpdatePlayerRatePun(RpcTarget.All, player, BankModel.SmallBlind);
            UpdatePlayerRatePun(RpcTarget.All, bigBlindPlayer, BankModel.BigBlind);
        }

        private async void FinishGame()
        {
            PlayerModel localModel = _matchModel.GetPlayerModel(PhotonNetwork.LocalPlayer);
            CombinationReader reader = new();
            Combination combination = await reader.GetCombination(localModel, _matchModel.TableCards);
            photonView.RPC("AddPlayerCombination", RpcTarget.All, PhotonNetwork.LocalPlayer, combination);
            await Task.Delay(_delayOperation);

            Player player = _matchModel.GetWinner();
            SendMessagePun("Winner: \n" + player.NickName, new ColorModel(Color.blue));
            await _bankPresenter.GiveAwayTheWinnings(_matchModel.GetPlayerModel(player));

            if (!localModel.Folded.Value)
            {
                for (int i = 0; i < localModel.Cards.Count; i++)
                    localModel.Cards[i].Showdown();
            }
            await Task.Delay(_delayOperation);
            NewDistribution();          
        }

        private void NewDistribution()
        {
            for (int i = 0; i < _matchModel.PlayersCount; i++)
                _matchModel.GetPlayerModel(_matchModel.CurrentPlayers[i])?.ResetModel();

            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.DestroyAll();


            _matchModel.SetMatchPhase(Phase.NewDistribution);
        }

        public void PlayerOutPun(Player player)
        {
            photonView.RPC("PlayerOut", player);
            photonView.RPC("RemovePlayer", RpcTarget.All, player);
        }

        public void UpdatePlayerRatePun(RpcTarget target, Player player, int newRate) => photonView.RPC("UpdatePlayerRate", target, player, newRate);

        [PunRPC]
        public void UpdatePlayerRate(Player player, int newRate)
        {
            PlayerModel playerModel = _matchModel.GetPlayerModel(player);
            int currentRate = playerModel.Rate.Value;
            playerModel.Raises(newRate);

            if (newRate + currentRate > BankModel.CurrentRate)
                _bankPresenter.ChangeRate(newRate + currentRate);

            if (newRate > 0)
                _bankPresenter.AddMoney(newRate);
        }

        public void SetPlayerStatePun(Player player, PlayerModel.PlayerState state) => photonView.RPC("SetPlayerState", RpcTarget.All, player, state);

        [PunRPC]
        public void SetPlayerState(Player player, PlayerModel.PlayerState state)
        {           
            PlayerModel model = _matchModel.GetPlayerModel(player);
            _playersInfo.GetValueOrDefault(player).ChangeColorText();
            model.CurrentState = state;
		}

        public void SetMatchPhasePun(Phase phase) => photonView.RPC("SetMatchPhase", RpcTarget.All, phase);

        [PunRPC]
        public void SetMatchPhase(Phase phase)
        {
            if (phase == Phase.NewDistributionAfterBet && _matchModel.DesiredCardCount > 5)
                _matchModel.SetMatchPhase(Phase.OpeningCard);
            else
                _matchModel.SetMatchPhase(phase);
        }

        [PunRPC]
        public void PlayerOut()
        {
            _matchModel.GetPlayerModel(PhotonNetwork.LocalPlayer).Folded.Value = true;
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }

        [PunRPC]
        public void AddTableCard(CardModel cardModel) => _matchModel.AddCardTable(cardModel);

        [PunRPC]
        public void AddPlayerCombination(Player player, Combination combination) => _matchModel.AddPlayerCombination(player, combination); 

		[PunRPC]
        public void AddPlayerModel(Player player, PlayerModel playerModel) => _matchModel.AddPlayerModel(player, playerModel);

        public void SendMessagePun(string message, ColorModel color) => photonView.RPC("SendEventMessage", RpcTarget.All, message, color);

        [PunRPC]
        public void SendEventMessage(string message, ColorModel color) => _matchView.DisplayMessage(message, color);

        [PunRPC]
        public void RemovePlayer(Player player) => _matchModel.RemovePlayer(player); 
	}
}
