using Bank;
using Players;
using PokerMatch;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Move = Players.PlayerModel.PlayerMove;
using State = Players.PlayerModel.PlayerState;
using UnityEngine;

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
        PlayerModel thisModel = (PlayerModel)datas[2];
        _matchPresenter.SetPlayerStatePun(player, State.Waiting);
        int playerIndex = _matchModel.CurrentPlayers.IndexOf(player);

		switch (currentMove)    
        {
            case Move.Fold:
                _matchPresenter.PlayerOutPun(player);
                thisModel.Folded.Value = true;
				playerIndex--;
				break;
            case Move.Call:
				if (BankModel.CurrentRate > thisModel.Money.Value)
                    goto case Move.AllIn;
                int rate = BankModel.CurrentRate - thisModel.Rate.Value;
                _matchPresenter.UpdatePlayerRatePun(player, rate);  
                thisModel.Raises(rate);
                break;    
            case Move.Raise:
                _matchPresenter.UpdatePlayerRatePun(player, thisModel.RaiseSum.Value);
                thisModel.Raises(thisModel.RaiseSum.Value);     
                break;
            case Move.AllIn:
				_matchPresenter.UpdatePlayerRatePun(player, thisModel.Money.Value);
                thisModel.Raises(thisModel.Money.Value);
                break;
        }

        Player nextPlayer = default;
        PlayerModel nextModel = default;

		for (int i = playerIndex + 1; i <= _matchModel.PlayersCount; i++)
        {
            nextPlayer = playerIndex + 1 >= _matchModel.PlayersCount ? _matchModel.CurrentPlayers[0] : _matchModel.CurrentPlayers[playerIndex + 1];
            nextModel = _matchModel.GetPlayerModel(nextPlayer);
            if((nextModel.Money.Value != 0 && !nextModel.Folded.Value) || nextModel.AllIn)
                break;

			playerIndex = _matchModel.CurrentPlayers.IndexOf(nextPlayer);
		}

		if(thisModel.Folded.Value && _matchModel.PlayersCount <= 1) 
        {
			_matchPresenter.SetMatchPhasePun(PokerMatchModel.MatchPhase.EndMatch);
            return;
		}

		if (nextPlayer == _matchModel.CurrentBetPlayer && (thisModel.Rate.Value == nextModel.Rate.Value ||  thisModel.Folded.Value || (thisModel.AllIn &&  (nextModel.Money.Value == BankModel.CurrentRate || nextModel.AllIn))))
        {
			_matchPresenter.SetMatchPhasePun(PokerMatchModel.MatchPhase.NewDistributionAfterBet);
			return;
		}    

		if (thisModel.Folded.Value && player == _matchModel.CurrentBetPlayer)
			_matchModel.CurrentBetPlayer = nextPlayer;

		_matchPresenter.SetPlayerStatePun(nextPlayer, State.Move);
    }

    public void RemoveCallback() => PhotonNetwork.RemoveCallbackTarget(this);   
}
