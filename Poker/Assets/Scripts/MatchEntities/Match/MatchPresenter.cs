using Bank;
using Cards;
using Players;
using Photon.Pun;
using UnityEngine;
using Combination;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Phase = PokerMatch.MatchModel.MatchPhase;

namespace PokerMatch  
{
    public class MatchPresenter : Presenter
    {
        private MatchView _matchView;
		private MatchService _matchService;
		private MatchModel _matchModel;

        private const int _timeOperation = 1000;
        private const int _timeBetweenMatch = 5000;  

        public MatchPresenter(MatchModel model, MatchView view, MatchService matchService) : base()
        {   
            _matchView = view;   
            _matchModel = model;
			_matchService = matchService;
              
			_matchModel.OnNewDistribution += NewDestribution;
			_matchModel.OnNewDistributionAfterBet += Dealer.TableDealing;
            _matchModel.OnBetSettings += StartBidding;
            _matchModel.OnEndedMatch += EndMatch;
            _matchModel.OnEndedGame += _matchView.OnEndMatch;

			Dealer.OnDealingEnded += EndDealing;
        }

        public async void StartMatch()   
        {
            SetPlayerInPlaces();
            _matchView.SetActiveView(true);
            await UniTask.Delay(_timeOperation);

            if(PhotonNetwork.IsMasterClient)
                _matchService.SetMatchPhasePun(Phase.NewDistribution);   
        }

        private async void NewDestribution(CardData cardData)    
        {
            SetBlindsButtom();
            if (!PhotonNetwork.IsMasterClient)
                return;
            await Dealer.StartDealing(cardData, _matchModel.CurrentPlayers);
            StartNewBidding();
        }  

		private void SetPlayerInPlaces()
        {
            _matchView.ShowPlayersInPlaces(_matchModel.GetPlayerInfo(PhotonNetwork.LocalPlayer), _matchModel.PokerPlayerData.LocalPosition);
			for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            {
                PlayerInfoView playerItem = _matchModel.GetPlayerInfo(PhotonNetwork.PlayerListOthers[i]);
                PlayerItemPosition position = _matchModel.PokerPlayerData.GetPosition(i);
                _matchView.ShowPlayersInPlaces(playerItem, position);
                _matchModel.AddPlayerItemPosition(PhotonNetwork.PlayerListOthers[i], position);
            }
        }

        private async void SetBlindsButtom()
        {
            Vector2 smallPosition = _matchModel.GetItemPosition(_matchModel.StartPlayer);
            Vector2 bigPosition = _matchModel.GetItemPosition(_matchModel.BigBlindPlayer);
            await _matchView.SetButtonBlind(smallPosition, bigPosition);  
        }

        private void EndDealing(List<CardModel> models)
        {
            for (int i = 0; i < models.Count; i++)
                _matchService.AddTableCard(models[i]);
            _matchService.SetMatchPhasePun(Phase.BetSetting);
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

            if (!PhotonNetwork.IsMasterClient)
                return;

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

        private void StartNewBidding()  
        {            
			_matchService.SetPlayerStatePun(_matchModel.StartPlayer, PlayerModel.PlayerState.Move);
            _matchService.UpdatePlayerRatePun(_matchModel.StartPlayer, BankModel.SmallBlind);
            _matchService.UpdatePlayerRatePun(_matchModel.BigBlindPlayer, BankModel.BigBlind, false);
        }

        private async void EndMatch() 
        {
			PlayerModel localModel = _matchModel.GetPlayerModel(PhotonNetwork.LocalPlayer);
			if (!localModel.Folded.Value)
			{
				CombinationModel combination = await CombinationReader.GetCombination(localModel, _matchModel.TableCards);
				_matchService.AddPlayerCombination(PhotonNetwork.LocalPlayer, combination);

				for (int i = 0; i < localModel.Cards.Count; i++)
					localModel.Cards[i].Showdown(); 
			}  

			if (!PhotonNetwork.IsMasterClient)
                return;  

			await UniTask.Delay(_timeOperation);

			Player winner = _matchModel.PlayersCount == 1 ? _matchModel.CurrentPlayers[0] : _matchModel.GetWinner();

            _matchService.ChangePlayerColor(winner, new ColorModel(new Color(1, 0.5f, 0)));
            _matchService.TransferMoneyToPlayer(winner);  

			await UniTask.Delay(_timeBetweenMatch); 

			_matchService.ChangePlayerColor(winner, new ColorModel(Color.white)); 
			PhotonNetwork.DestroyAll(); 
            _matchService.SetMatchPhasePun(Phase.NewDistribution);
		}

        public override void Dispose()  
        {
            _matchModel.OnNewDistribution -= NewDestribution;
            _matchModel.OnNewDistributionAfterBet -= Dealer.TableDealing;
            _matchModel.OnBetSettings -= StartBidding;
            _matchModel.OnEndedMatch -= EndMatch;
            _matchModel.OnEndedGame -= _matchView.OnEndMatch;
            Dealer.OnDealingEnded -= EndDealing;
        }
    }
}