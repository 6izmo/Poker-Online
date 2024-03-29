using TMPro;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _contentView;
    [SerializeField] private RoomItem _roomItemPrefab;

    private List<RoomItem> _roomItems = new();
    private List<RoomInfo> _allRoomsInfo = new();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                int index = _roomItems.FindIndex(x => x.RoomInfo.Name == roomInfo.Name);
                if(index != -1)
                {
                    Destroy(_roomItems[index].gameObject);
                    _roomItems.RemoveAt(index);
                }
            }
            else
            {
                for (int i = 0; i < _roomItems.Count; i++)
                {
                    if (_allRoomsInfo[i].masterClientId == roomInfo.masterClientId)
                        return;
                }

                RoomItem roomItem = Instantiate(_roomItemPrefab, _contentView);
                if (roomItem != null)
                {
                    roomItem.SetRoomInfo(roomInfo);
                    _allRoomsInfo.Add(roomInfo);
                    _roomItems.Add(roomItem);
                }
            }
        }
    }
}
