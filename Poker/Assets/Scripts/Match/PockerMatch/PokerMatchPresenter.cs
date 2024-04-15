using Bank;
using Cards;
using Players;
using Photon.Pun;
using UnityEngine;
using Combination;
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
        private const int _delayBetweenMatch = 5000;

        public void Init(PokerMatchModel model, PokerMatchView view, BankPresenter bankPreseter, Dealer dealer)
        {
            _dealer = dealer;
            _matchView = view;
            _matchModel = model;
            _bankPresenter = bankPreseter;

            _matchModel.OnNewDistribution += NewMatch;
			_matchModel.OnNewDistributionAfterBet += _dealer.TableDealing;
            _matchModel.OnBetSettings += StartBidding;
            _matchModel.OnEndedMatch += EndMatch;
            _matchModel.OnEndedGame += _matchView.OnEndMatch;
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

        private void NewMatch(CardData cardData)
        {
			_matchView.SetActiveCardDeck(true);
			StartNewBidding();
			_dealer.StartDealing(cardData, _matchModel.CurrentPlayers);
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
            int allIn = 0;
			for (int i = 0; i < _matchModel.PlayersCount; i++)
			{
				PlayerModel playerModel = _matchModel.GetPlayerModel(_matchModel.CurrentPlayers[i]);
				playerModel.Rate.Value = 0;
                if (playerModel.AllIn)
					allIn++;
			}

			_bankPresenter.ChangeRate(0);
			if (!PhotonNetwork.IsMasterClient)
				return;

			if (allIn == _matchModel.PlayersCount - 1)
            {
                if(_matchModel.DesiredCardCount > 5)
                {
					SetMatchPhasePun(Phase.EndMatch);
					return;
                }
				SetMatchPhasePun(Phase.NewDistributionAfterBet);
				return;
            }

			SetPlayerStatePun(_matchModel.StartPlayer, PlayerModel.PlayerState.Move);   
        }

        private void StartNewBidding()
        {       
            if (!PhotonNetwork.IsMasterClient)
                return;

            SetPlayerStatePun(_matchModel.StartPlayer, PlayerModel.PlayerState.Move);
            SetBlinds(_matchModel.StartPlayer);
        }

        private void SetBlinds(Player player)
        {
            Player bigBlindPlayer = player.ActorNumber == PhotonNetwork.PlayerList.Length ? PhotonNetwork.PlayerList[0] : PhotonNetwork.PlayerList[player.ActorNumber];
            UpdatePlayerRatePun(player, BankModel.SmallBlind);
            UpdatePlayerRatePun(bigBlindPlayer, BankModel.BigBlind, false);
        }

        private async void EndMatch()
        {
            Player winner;
			PlayerModel localModel = _matchModel.GetPlayerModel(PhotonNetwork.LocalPlayer);

			if (_matchModel.PlayersCount == 1)
				winner = _matchModel.CurrentPlayers[0];
            else
            {
                if(!localModel.Folded.Value)
                {
					CombinationModel combination = await new CombinationReader().GetCombination(localModel, _matchModel.TableCards);
					photonView.RPC("AddPlayerCombination", RpcTarget.All, PhotonNetwork.LocalPlayer, combination);
				}
				await Task.Delay(_delayOperation);
				winner = _matchModel.GetWinner();
			}

            PlayerModel winnerModel = _matchModel.GetPlayerModel(winner);
            PlayerInfoView info = _playersInfo.GetValueOrDefault(winner);
			info.ChangeColorText(new Color(1, 0.5f, 0));

			await _bankPresenter.GiveAwayTheWinnings(winnerModel);

            if (!localModel.Folded.Value)
            {
                for (int i = 0; i < localModel.Cards.Count; i++)
                    localModel.Cards[i].Showdown();
            }
            await Task.Delay(_delayBetweenMatch);
			info.ChangeColorText(Color.white);

            if(localModel.Money.Value == 0)
                PlayerOutPun(PhotonNetwork.LocalPlayer);

			if (PhotonNetwork.IsMasterClient)
				PhotonNetwork.DestroyAll();

			_matchModel.SetMatchPhase(Phase.NewDistribution);
		}

        public void PlayerOutPun(Player player) => photonView.RPC("PlayerOut", RpcTarget.All, player);  

        public void UpdatePlayerRatePun(Player player, int newRate, bool canChangePlayer = true) 
            => photonView.RPC("UpdatePlayerRate", RpcTarget.All, player, newRate, canChangePlayer);   

        [PunRPC]
        public void UpdatePlayerRate(Player player, int newRate, bool canChangePlayer = true)
        {
            PlayerModel playerModel = _matchModel.GetPlayerModel(player);
            int currentRate = playerModel.Rate.Value;
            playerModel.Raises(newRate);

            if (newRate + currentRate > BankModel.CurrentRate)
            {
				_bankPresenter.ChangeRate(newRate + currentRate);
                if (canChangePlayer)
                    _matchModel.CurrentBetPlayer = player;
			}

            if (newRate > 0)
                _bankPresenter.AddMoney(newRate);
        }

        public void SetPlayerStatePun(Player player, PlayerModel.PlayerState state) => photonView.RPC("SetPlayerState", RpcTarget.All, player, state);

        [PunRPC]
        public void SetPlayerState(Player player, PlayerModel.PlayerState state)
        {                      
            PlayerModel model = _matchModel.GetPlayerModel(player);
            _playersInfo.GetValueOrDefault(player).ChangeColorText(Color.green);
            model.CurrentState = state;
		}

        public void SetMatchPhasePun(Phase phase) => photonView.RPC("SetMatchPhase", RpcTarget.All, phase);

        [PunRPC]
        public void SetMatchPhase(Phase phase)
        {
            if (phase == Phase.NewDistributionAfterBet && _matchModel.DesiredCardCount > 5)
                _matchModel.SetMatchPhase(Phase.EndMatch);
            else
                _matchModel.SetMatchPhase(phase);
        }

        [PunRPC]
        public void PlayerOut(Player player)
        {
            _matchModel.GetPlayerModel(player).Folded.Value = true;
            PhotonNetwork.DestroyPlayerObjects(player);
			_matchModel.RemovePlayer(player);
		}

        [PunRPC]
        public void AddTableCard(CardModel cardModel) => _matchModel.AddCardTable(cardModel);

        [PunRPC]
        public void AddPlayerCombination(Player player, CombinationModel combination) => _matchModel.AddPlayerCombination(player, combination); 

		[PunRPC]
        public void AddPlayerModel(Player player, PlayerModel playerModel) => _matchModel.AddPlayerModel(player, playerModel);

	}
}