using System;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlayerList
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerListPresenter : MonoBehaviourPunCallbacks
    {
        private PlayerListModel _playerListModel;
        private PlayerListView _playerListView;

		public event Action<Dictionary<Player, PlayerInfoView>> OnAllPlayersReady;

        public void Init(PlayerListModel model, PlayerListView view)
        {
            _playerListModel = model;
			_playerListView = view;

			_playerListView.OnLeaved += Leave;
        }

        private void Start()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
                _playerListModel.AddPlayer(player);
        }

        public async void UpdateReadyCondition(bool condition)
        {
            photonView.RPC("RpcSetReadyCondition", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer, condition);
            await Task.Delay(300);
            photonView.RPC("CheckReadyConditions", RpcTarget.All);
        }

        private void Leave() => PhotonNetwork.LeaveRoom();

        [PunRPC]
        public void CheckReadyConditions()
        {
            if (!_playerListModel.GetPlayersReadyCondition())
                return;

            OnAllPlayersReady?.Invoke(_playerListModel.PlayerItems);
        }

        [PunRPC]
        public void RpcSetReadyCondition(Player player, bool condition)
        {
            PlayerInfoView playerItem = _playerListModel.PlayerItems.GetValueOrDefault(player);
            if (playerItem != null)
                playerItem.SetReadyCondition(condition);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) => _playerListModel.AddPlayer(newPlayer);

        public override void OnPlayerLeftRoom(Player otherPlayer) => _playerListModel.RemovePLayer(otherPlayer);

        private void OnDestroy() => _playerListView.OnLeaved -= Leave;
    }
}
