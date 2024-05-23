using Photon.Realtime;
using System.Collections.Generic;

namespace PlayerList
{
    public class PlayerListModel
    {
        private PlayerListView _view;
        private PlayerInfoView _playerItemPrefab;

        private Dictionary<Player, PlayerInfoView> _playerItems = new();

        public Dictionary<Player, PlayerInfoView> PlayerItems => _playerItems;

        public PlayerListModel(PlayerListView view, PlayerInfoView playerItemPrefab)
        {
            _view = view;
            _playerItemPrefab = playerItemPrefab;
        }

        public void AddPlayer(Player player)
        {
			PlayerInfoView playerItem = _view.ShowPlayerToPlayerPanel(_playerItemPrefab);
            playerItem.SetPLayerInfo(player);
            _playerItems.Add(player, playerItem);
        }

        public void RemovePLayer(Player player)
        {
            PlayerInfoView playerItem = _playerItems.GetValueOrDefault(player);
            _playerItems.Remove(player);
            _view.RemovePlayerFromPlayerList(playerItem);
        }

        public bool GetPlayersReadyCondition()
        {
            foreach (PlayerInfoView playerItem in _playerItems.Values)
            {
                if (!playerItem.Ready)
                    return false;
            }
            _view.HideList();
            return true;
        }
    }
}
