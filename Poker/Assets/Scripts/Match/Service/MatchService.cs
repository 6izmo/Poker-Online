using Bank;
using Cards;
using Players;
using PokerMatch;
using Photon.Pun;
using Combination;
using UnityEngine;
using Photon.Realtime;
using Phase = PokerMatch.MatchModel.MatchPhase;

public class MatchService : MonoBehaviourPunCallbacks
{
	private MatchModel _matchModel;
	private BankPresenter _bankPresenter;

	public void Init(MatchModel pokerMatchModel, BankPresenter bankPresenter)
	{
		_matchModel = pokerMatchModel; 
		_bankPresenter = bankPresenter;
	}

    public override void OnPlayerLeftRoom(Player otherPlayer)  
    {
        base.OnPlayerLeftRoom(otherPlayer);
		SetMatchPhasePun(Phase.NewDistribution);  
    }

    public void SetMatchPhasePun(Phase phase) => photonView.RPC("SetMatchPhase", RpcTarget.All, phase);

	[PunRPC]
	void SetMatchPhase(Phase phase)
	{
		switch (phase) 
		{
			case Phase.BetSetting:
                _bankPresenter.ChangeRate(0);
                break;
			case Phase.NewDistributionAfterBet:
				if (_matchModel.DesiredCardCount > 5)
					phase = Phase.EndMatch;
                break;
		}
		_matchModel.SetMatchPhase(phase);
	} 

    public void AddPlayerModel(PlayerModel playerModel) => photonView.RPC("AddPlayerModelPun", RpcTarget.All, PhotonNetwork.LocalPlayer, playerModel);

	[PunRPC]
	void AddPlayerModelPun(Player player, PlayerModel playerModel) => _matchModel.AddPlayerModel(player, playerModel);

	public void AddPlayerCombination(Player player, CombinationModel combination) 
		=> photonView.RPC("AddPlayerCombinationPun", RpcTarget.All, player, combination);

	[PunRPC]
	void AddPlayerCombinationPun(Player player, CombinationModel combination) => _matchModel.AddCombination(player, combination);

	public void AddTableCard(CardModel cardModel) => photonView.RPC("AddTableCardPun", RpcTarget.All, cardModel);

	[PunRPC]
	void AddTableCardPun(CardModel cardModel) => _matchModel.AddCardTable(cardModel);

	public void SetStartPlayer() => photonView.RPC("SetStartPlayerPun", RpcTarget.All);

	[PunRPC]
	void SetStartPlayerPun() => _matchModel.SetStartPlayer();

	public void PlayerOut(Player player) => photonView.RPC("PlayerOutPun", RpcTarget.All, player);

	[PunRPC]
	void PlayerOutPun(Player player)
	{
		_matchModel.GetPlayerModel(player).Folded.Value = true;
		_matchModel.RemovePlayer(player); 
	}

	public void SetPlayerStatePun(Player player, PlayerModel.PlayerState state) => photonView.RPC("SetPlayerState", RpcTarget.All, player, state);

	[PunRPC]
	void SetPlayerState(Player player, PlayerModel.PlayerState state)
	{
		PlayerModel model = _matchModel.GetPlayerModel(player);
		Color color = state == PlayerModel.PlayerState.Move ? Color.green : Color.white;
		_matchModel.GetPlayerInfo(player).ChangeColorText(color);
		model.CurrentState = state;
	}

	public void UpdatePlayerRatePun(Player player, int newRate, bool canChangePlayer = true)
		=> photonView.RPC("UpdatePlayerRate", RpcTarget.All, player, newRate, canChangePlayer);

	[PunRPC]
	void UpdatePlayerRate(Player player, int newRate, bool canChangePlayer = true)
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

	public void ChangePlayerColor(Player player, ColorModel model) => photonView.RPC("ChangePlayerColorPun", RpcTarget.All, player, model);

	[PunRPC]
	void ChangePlayerColorPun(Player player, ColorModel colorModel)
	{
		PlayerInfoView view = _matchModel.GetPlayerInfo(player);
		view.ChangeColorText(colorModel.Color);
	}

	public void GiveAwayTheWinnings(Player player) => photonView.RPC("GiveAwayTheWinningsPun", RpcTarget.All, player);  

	[PunRPC]
	void GiveAwayTheWinningsPun(Player player)
	{
		PlayerModel winnerModel = _matchModel.GetPlayerModel(player);
		_bankPresenter.GiveAwayTheWinnings(winnerModel);
	}
}
