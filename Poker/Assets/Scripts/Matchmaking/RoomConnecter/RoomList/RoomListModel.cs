using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

namespace RoomList
{
    public class RoomListModel
    {
        private List<RoomItem> _roomItems = new();
        private List<RoomInfo> _allRoomsInfo = new();

        public List<RoomItem> RoomItems => _roomItems;
        public List<RoomInfo> RoomsInfo => _allRoomsInfo;

        public RoomItem RoomPrefab { get; private set; }

        public RoomListModel(RoomItem itemPrefab) => RoomPrefab = itemPrefab;

        public void CreateRoom(string name)
        {
            if (!PhotonNetwork.IsConnected)
                return;

            RoomOptions roomOptions = new()
            {
                MaxPlayers = 4,
                IsVisible = true,   
            };

            PhotonNetwork.CreateRoom(name, roomOptions, TypedLobby.Default);
        }

        public bool RoomCreated(string room) => _roomItems.Any(x => x.RoomInfo.Name == room);

        public void AddRoom(RoomInfo roomInfo, RoomItem roomItem)
        {
            roomItem.SetRoomInfo(roomInfo);
            _allRoomsInfo.Add(roomInfo);
            _roomItems.Add(roomItem);
        }

        public void RemoveRoomItem(RoomItem item)
        {
            if(_roomItems.Contains(item))
                _roomItems.Remove(item); 
        }
    }
}
