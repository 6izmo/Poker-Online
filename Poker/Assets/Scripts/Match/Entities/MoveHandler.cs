using Bank;
using Players;
using PokerMatch;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Move = Players.PlayerModel.PlayerMove;
using State = Players.PlayerModel.PlayerState;

public class MoveHandler : IOnEventCallback
{
    private MatchService _matchService;
    private MatchModel _matchModel;

	public MoveHandler(MatchService matchService, MatchModel model)
    {
        _matchService = matchService;
        _matchModel = model;

		PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code != (int)EventCode.Move || !PhotonNetwork.IsMasterClient)   
            return;

        object[] datas = (object[])photonEvent.CustomData;  
        Player player = (Player)datas[0];
        Move currentMove = (Move)datas[1];
        PlayerModel handlerModel = (PlayerModel)datas[2];
        _matchService.SetPlayerStatePun(player, State.Waiting);
        int playerIndex = _matchModel.CurrentPlayers.IndexOf(player);

		switch (currentMove)     
        {
            case Move.Fold:
				_matchService.PlayerOut(player);
                handlerModel.Folded.Value = true;
				playerIndex--;
				break;
            case Move.Call:
				if (BankModel.CurrentRate > handlerModel.Money.Value)
                    goto case Move.AllIn;
                int rate = BankModel.CurrentRate - handlerModel.Rate.Value;
                _matchService.UpdatePlayerRatePun(player, rate);  
                handlerModel.Raises(rate);
                break;    
            case Move.Raise:
                _matchService.UpdatePlayerRatePun(player, handlerModel.RaiseSum.Value);
                handlerModel.Raises(handlerModel.RaiseSum.Value);     
                break;
            case Move.AllIn:
				_matchService.UpdatePlayerRatePun(player, handlerModel.Money.Value);
                handlerModel.Raises(handlerModel.Money.Value);
                break;   
        }

        Player nextPlayer = default;
        PlayerModel nextModel = default;

		for (int i = playerIndex + 1; i <= _matchModel.PlayersCount; i++)
        {
            nextPlayer = playerIndex + 1 >= _matchModel.PlayersCount ? _matchModel.CurrentPlayers[0] : _matchModel.CurrentPlayers[playerIndex + 1];
            nextModel = _matchModel.GetPlayerModel(nextPlayer);
            if((nextModel.Money.Value != 0 && !nextModel.Folded.Value) || (nextModel.AllIn && nextPlayer == _matchModel.CurrentBetPlayer))
                break;

			playerIndex = _matchModel.CurrentPlayers.IndexOf(nextPlayer);
		}

		if(handlerModel.Folded.Value && _matchModel.PlayersCount <= 1) 
        {
			_matchService.SetMatchPhasePun(MatchModel.MatchPhase.EndMatch);
            return;
		}

		if (nextPlayer == _matchModel.CurrentBetPlayer && (handlerModel.Rate.Value == nextModel.Rate.Value ||  handlerModel.Folded.Value ||
            (handlerModel.AllIn && (nextModel.Money.Value == BankModel.CurrentRate || nextModel.AllIn))))
        {
			_matchService.SetMatchPhasePun(MatchModel.MatchPhase.NewDistributionAfterBet);
			return;
		}    

		if (handlerModel.Folded.Value && player == _matchModel.CurrentBetPlayer)   
			_matchModel.CurrentBetPlayer = nextPlayer; 

		_matchService.SetPlayerStatePun(nextPlayer, State.Move);
    }

    public void RemoveCallback() => PhotonNetwork.RemoveCallbackTarget(this);   
}
