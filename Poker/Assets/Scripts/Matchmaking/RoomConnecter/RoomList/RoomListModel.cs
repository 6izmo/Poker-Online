using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using System.Collections.Generic;

namespace RoomList
{
    public class RoomListModel
    {
        private Dictionary<RoomInfo, RoomItem> _rooms = new();

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

        public RoomItem GetRoomItem(RoomInfo info) => _rooms.GetValueOrDefault(info);

        public bool IsRoomCreated(string room) => _rooms.Keys.Any(x => x.Name == room);

        public void AddRoom(RoomInfo roomInfo, RoomItem roomItem)
        {
            roomItem.SetRoomInfo(roomInfo);
            _rooms.Add(roomInfo, roomItem);
        }

        public void RemoveRoomItem(RoomInfo info)
        {
            if(_rooms.ContainsKey(info))
                _rooms.Remove(info); 
        }
    }
}
