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

	public void SetMatchPhasePun(Phase phase) => photonView.RPC("SetMatchPhase", RpcTarget.All, phase);

	[PunRPC]
	public void SetMatchPhase(Phase phase)
	{
		if (phase == Phase.NewDistributionAfterBet && _matchModel.DesiredCardCount > 5)
			_matchModel.SetMatchPhase(Phase.EndMatch);
		else
			_matchModel.SetMatchPhase(phase);
	}

	public void AddPlayerModel(PlayerModel playerModel) => photonView.RPC("AddPlayerModelPun", RpcTarget.All, PhotonNetwork.LocalPlayer, playerModel);

	[PunRPC]
	public void AddPlayerModelPun(Player player, PlayerModel playerModel) => _matchModel.AddPlayerModel(player, playerModel);

	public void AddPlayerCombination(Player player, CombinationModel combination) 
		=> photonView.RPC("AddPlayerCombinationPun", RpcTarget.All, player, combination);

	[PunRPC]
	public void AddPlayerCombinationPun(Player player, CombinationModel combination) => _matchModel.AddCombination(player, combination);

	public void AddTableCard(CardModel cardModel) => photonView.RPC("AddTableCardPun", RpcTarget.All, cardModel);

	[PunRPC]
	public void AddTableCardPun(CardModel cardModel) => _matchModel.AddCardTable(cardModel);

	public void SetStartPlayer() => photonView.RPC("SetStartPlayerPun", RpcTarget.All);

	[PunRPC]
	public void SetStartPlayerPun() => _matchModel.SetStartPlayer();

	public void PlayerOut(Player player) => photonView.RPC("PlayerOutPun", RpcTarget.All, player);

	[PunRPC]
	public void PlayerOutPun(Player player)
	{
		_matchModel.GetPlayerModel(player).Folded.Value = true;
		PhotonNetwork.DestroyPlayerObjects(player);
		_matchModel.RemovePlayer(player);
	}

	public void SetPlayerStatePun(Player player, PlayerModel.PlayerState state) => photonView.RPC("SetPlayerState", RpcTarget.All, player, state);

	[PunRPC]
	public void SetPlayerState(Player player, PlayerModel.PlayerState state)
	{
		PlayerModel model = _matchModel.GetPlayerModel(player);
		Color color = state == PlayerModel.PlayerState.Move ? Color.green : Color.white;
		_matchModel.GetPlayerInfo(player).ChangeColorText(color);
		model.CurrentState = state;
	}

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
}
