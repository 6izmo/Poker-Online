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
    private PokerMatchPresenter _matchPresenter;
    private PokerMatchModel _matchModel;

	public MoveHandler(PokerMatchPresenter presenter, PokerMatchModel model)
    {
        _matchPresenter = presenter;
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
        PlayerModel localModel = (PlayerModel)datas[2];
        _matchPresenter.SetPlayerStatePun(player, State.Waiting);
        int playerIndex = _matchModel.CurrentPlayers.IndexOf(player);
        
		switch (currentMove)    
        {
            case Move.Fold:
                _matchPresenter.PlayerOutPun(player);
                localModel.Folded.Value = true;
				playerIndex--;
				break;
            case Move.Call:
                int rate = BankModel.CurrentRate - localModel.Rate.Value;
                _matchPresenter.UpdatePlayerRatePun(player, rate);  
                localModel.Raises(rate);
                break;    
            case Move.Raise:
                _matchPresenter.UpdatePlayerRatePun(player, localModel.RaiseSum.Value);
                localModel.Raises(localModel.RaiseSum.Value);
                break;
            case Move.AllIn:
                _matchPresenter.UpdatePlayerRatePun(player, localModel.Money.Value);
                localModel.Raises(localModel.Money.Value);
                break;
        }

        Player nextPlayer = default;
        PlayerModel nextPlayerModel = default;

        for (int i = 0; i < _matchModel.PlayersCount; i++)
        {
            if (_matchModel.CurrentPlayers[i] == player)
                continue;

            nextPlayer = playerIndex + 1 >= _matchModel.PlayersCount ? _matchModel.CurrentPlayers[0] : _matchModel.CurrentPlayers[playerIndex + 1];
            nextPlayerModel = _matchModel.GetPlayerModel(nextPlayer);

            if(nextPlayerModel.Money.Value != 0 || !nextPlayerModel.Folded.Value)
                break;

			playerIndex = _matchModel.CurrentPlayers.IndexOf(nextPlayer);
		}

		if(localModel.Folded.Value && _matchModel.PlayersCount <= 1)
        {
			_matchPresenter.SetMatchPhasePun(PokerMatchModel.MatchPhase.EndMatch);
            return;
		}

		if (nextPlayer == _matchModel.CurrentBetPlayer && (localModel.Rate.Value == nextPlayerModel.Rate.Value ||  localModel.Folded.Value))
        {
			_matchPresenter.SetMatchPhasePun(PokerMatchModel.MatchPhase.NewDistributionAfterBet);
			return;
		}

		if (localModel.Folded.Value && player == _matchModel.CurrentBetPlayer)
			_matchModel.CurrentBetPlayer = nextPlayer;

		_matchPresenter.SetPlayerStatePun(nextPlayer, State.Move);
    }

    public void RemoveCallback() => PhotonNetwork.RemoveCallbackTarget(this);   
}
