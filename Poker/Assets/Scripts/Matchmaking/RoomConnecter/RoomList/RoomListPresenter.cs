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
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList)
                {
                    int index = _listModel.RoomItems.FindIndex(x => x.RoomInfo.Name == roomInfo.Name);
                    if (index != -1)
                    {
                        RoomItem item = _listModel.RoomItems[index]; 
                        Destroy(item.gameObject);
                        _listModel.RemoveRoomItem(item);
                    }
                }
                else
                {
                    for (int i = 0; i < _listModel.RoomItems.Count; i++)
                    {
                        if (_listModel.RoomsInfo[i].masterClientId == roomInfo.masterClientId)
                            return;
                    }

                    RoomItem roomItem = _listView.ShowRoom(_listModel.RoomPrefab);
                    if (roomItem != null)
                        _listModel.AddRoom(roomInfo, roomItem);
                }
            }
        }

        private void JoinByName(string name)
        {
            if(_listModel.RoomCreated(name))  
                PhotonNetwork.JoinRoom(name);
        }

        private void Exit() => SceneTransition.SwitchToScene("Menu");

		private void OnDestroy()
		{
			_listView.OnRoomCreated -= _listModel.CreateRoom;
			_listView.OnExit -= Exit;
			_listView.OnRoomJoined -= JoinByName;
		}
	}
}
