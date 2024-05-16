using Bank;
using Cards;
using Players;
using Photon.Pun;
using UnityEngine;
using Combination;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;
using Phase = PokerMatch.MatchModel.MatchPhase;

namespace PokerMatch
{
    public class MatchPresenter
    {
        private MatchView _matchView;
		private MatchService _matchService;
		private MatchModel _matchModel;
        private BankPresenter _bankPresenter;

        private const int _delayOperation = 1000;
        private const int _delayBetweenMatch = 5000;

        public MatchPresenter(MatchModel model, MatchView view, MatchService matchService, BankPresenter bankPreseter)
        {
            _matchView = view;
            _matchModel = model;
			_matchService = matchService;
			_bankPresenter = bankPreseter;

			_matchModel.OnNewDistribution += NewMatch;
			_matchModel.OnNewDistributionAfterBet += Dealer.TableDealing;
            _matchModel.OnBetSettings += StartBidding;
            _matchModel.OnEndedMatch += EndMatch;
            _matchModel.OnEndedGame += _matchView.OnEndMatch;

			Dealer.OnDealingEnded += EndDealing;
        }

        public async void StartMatch(PlayerModel model)
        {
            _matchService.AddPlayerModel(model);
            _bankPresenter.ActivateBank();
            SetPlayerInPlaces();
            await Task.Delay(_delayOperation);
            _matchModel.SetMatchPhase(Phase.NewDistribution);  
        }

        private void NewMatch(CardData cardData)
        {
			_matchView.SetActiveCardDeck(true);
			StartNewBidding();
			Dealer.StartDealing(cardData, _matchModel.CurrentPlayers);
        }

		private void SetPlayerInPlaces()
        {
            _matchView.ShowPlayersInPlaces(_matchModel.GetPlayerInfo(PhotonNetwork.LocalPlayer), _matchModel.PokerPlayerData.LocalPosition);
			for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            {
                PlayerInfoView playerItem = _matchModel.GetPlayerInfo(PhotonNetwork.PlayerListOthers[i]);
                PlayerItemPosition position = _matchModel.PokerPlayerData.GetPosition(i);
                _matchView.ShowPlayersInPlaces(playerItem, position);
            }
        }

        private void EndDealing(List<CardModel> models)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < models.Count; i++)
                    _matchService.AddTableCard(models[i]);
            }
            _matchModel.SetMatchPhase(Phase.BetSetting);
		}

        private void StartBidding()
        {
            int allIn = 0;
			for (int i = 0; i < _matchModel.PlayersCount; i++)
			{
				PlayerModel playerModel = _matchModel.GetPlayerModel(_matchModel.CurrentPlayers[i]);
				PlayerInfoView info = _matchModel.GetPlayerInfo(_matchModel.CurrentPlayers[i]);
                info.ChangeColorText(Color.white);
				playerModel.Rate.Value = 0;
				if (playerModel.AllIn)
					allIn++;
			}

			_bankPresenter.ChangeRate(0);
			if (PhotonNetwork.IsMasterClient)
            {
				if (allIn >= _matchModel.PlayersCount - 1)
				{
					if (_matchModel.DesiredCardCount > 5)
					{
						_matchService.SetMatchPhasePun(Phase.EndMatch);
						return;
					}
					_matchService.SetMatchPhasePun(Phase.NewDistributionAfterBet);
					return;
				}
                _matchService.SetStartPlayer();
			    _matchService.SetPlayerStatePun(_matchModel.StartPlayer, PlayerModel.PlayerState.Move);
			}
        }   

        private void StartNewBidding()
        {       
            if (PhotonNetwork.IsMasterClient)
            {
				_matchService.SetPlayerStatePun(_matchModel.StartPlayer, PlayerModel.PlayerState.Move);
				SetBlinds(_matchModel.StartPlayer);
			}
        }

        private void SetBlinds(Player player)
        {
            Player bigBlindPlayer = player.ActorNumber == PhotonNetwork.PlayerList.Length ? PhotonNetwork.PlayerList[0] : PhotonNetwork.PlayerList[player.ActorNumber];
			_matchService.UpdatePlayerRatePun(player, BankModel.SmallBlind);
			_matchService.UpdatePlayerRatePun(bigBlindPlayer, BankModel.BigBlind, false);
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
                    _matchService.AddPlayerCombination(PhotonNetwork.LocalPlayer, combination);
				}
				await Task.Delay(_delayOperation);
				winner = _matchModel.GetWinner();
			}

            PlayerModel winnerModel = _matchModel.GetPlayerModel(winner);
            PlayerInfoView info = _matchModel.GetPlayerInfo(winner);
			info.ChangeColorText(new Color(1, 0.5f, 0));

			await _bankPresenter.GiveAwayTheWinnings(winnerModel);

            if (!localModel.Folded.Value)
            {
                for (int i = 0; i < localModel.Cards.Count; i++)
                    localModel.Cards[i].Showdown();
            }
            await Task.Delay(_delayBetweenMatch);

            if(localModel.Money.Value == 0)
                _matchService.PlayerOutPun(PhotonNetwork.LocalPlayer);

			if (PhotonNetwork.IsMasterClient)
				PhotonNetwork.DestroyAll();

			_matchModel.SetMatchPhase(Phase.NewDistribution);
		}
	}
}