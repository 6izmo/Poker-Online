using Bank;
using System;
using Players;
using PokerMatch;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Move = Players.PlayerModel.PlayerMove;
using State = Players.PlayerModel.PlayerState;
using UnityEngine;

public class MoveHandler : IOnEventCallback, IDisposable
{
    private PokerMatchPresenter _matchPresenter;
    private PokerMatchModel _matchModel;
    private int _moveCount = 0;

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
        _moveCount++;

		switch (currentMove)    
        {
            case Move.Fold:
                playerIndex -= 1;
                _moveCount -= 1;
                _matchPresenter.PlayerOutPun(player);
                localModel.Folded.Value = true;
				break;
            case Move.Call:
                int rate = BankModel.CurrentRate - localModel.Rate.Value;
                _matchPresenter.UpdatePlayerRatePun(RpcTarget.All, player, rate);
                localModel.Raises(rate);
                break;    
            case Move.Raise:
                _matchPresenter.UpdatePlayerRatePun(RpcTarget.All, player, localModel.RaiseSum.Value);
                localModel.Raises(localModel.RaiseSum.Value);
                break;
            case Move.AllIn:
                _matchPresenter.UpdatePlayerRatePun(RpcTarget.All, player, localModel.Money.Value);
                _matchPresenter.SendMessagePun("ALL IN", new ColorModel(Color.yellow));
                localModel.Raises(localModel.Money.Value);
                break;
        }

		Player nextPlayer = playerIndex + 1 >= _matchModel.PlayersCount ? _matchModel.CurrentPlayers[0] : _matchModel.CurrentPlayers[playerIndex + 1];
		PlayerModel nextPlayerModel = _matchModel.GetPlayerModel(nextPlayer);

		if (((localModel.Rate.Value == nextPlayerModel.Rate.Value) && (_moveCount == _matchModel.PlayersCount)) 
            || ((localModel.Rate.Value == nextPlayerModel.Rate.Value) && (_moveCount >= _matchModel.PlayersCount)) 
            || (_moveCount >= _matchModel.PlayersCount && localModel.Folded.Value))
        {
            _moveCount = 0;
            _matchPresenter.SetMatchPhasePun(PokerMatchModel.MatchPhase.NewDistributionAfterBet);
            return;
        }
        _matchPresenter.SetPlayerStatePun(nextPlayer, State.Move);
    }

    public void Dispose() => PhotonNetwork.RemoveCallbackTarget(this);
}
