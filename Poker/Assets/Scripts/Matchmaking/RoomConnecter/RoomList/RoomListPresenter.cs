using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace RoomList
{
    public class RoomListPresenter : MonoBehaviourPunCallbacks
    {
        private RoomListModel _listModel;
        private RoomListView _listView;

        public void Init(RoomListModel listModel, RoomListView listView)  
        {
            _listModel = listModel;
            _listView = listView;

            _listView.OnRoomCreated += _listModel.CreateRoom;
            _listView.OnExit += Exit;
            _listView.OnRoomJoined += JoinByName;
            _listView.OnRoomRandomJoined += JoinRandom;

            PhotonConnecter.Instance.OnRoomListUpdated += UpdateRooms;
            UpdateRooms(PhotonConnecter.Instance.RoomList);
        }

        private void UpdateRooms(List<RoomInfo> roomList)  
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList)
                {
                    RoomItem item = _listModel.GetRoomItem(roomInfo);
                    Destroy(item.gameObject);
                    _listModel.RemoveRoomItem(roomInfo);
                }
                else
                {
                    if (_listModel.IsRoomCreated(roomInfo.Name))
                    {
                        RoomItem room = _listModel.GetRoomItem(roomInfo);
                        room.SetRoomInfo(roomInfo);
                        return;
                    }
                    else
                    {
                        RoomItem roomItem = _listView.ShowRoom(_listModel.RoomPrefab);
                        if (roomItem != null)
                            _listModel.AddRoom(roomInfo, roomItem);
                    }
                }
            }
        }

        private void JoinByName(string name)
        {
            if(_listModel.IsRoomCreated(name))  
                PhotonNetwork.JoinRoom(name);
        }

        private void JoinRandom() => PhotonNetwork.JoinRandomRoom();

        private void Exit() => SceneTransition.SwitchToScene("Menu");

        private void OnDestroy()
        {
            _listView.OnRoomCreated -= _listModel.CreateRoom;
            _listView.OnExit -= Exit;
            _listView.OnRoomJoined -= JoinByName; 
            _listView.OnRoomRandomJoined -= JoinRandom;
            PhotonConnecter.Instance.OnRoomListUpdated -= UpdateRooms;
        }
    }
}
