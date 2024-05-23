using System;
using Utilities;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;

public class PhotonConnecter : PersistentSingletonPun<PhotonConnecter> 
{
    [SerializeField] private RegionToken _regionToken;

    private List<RoomInfo> _roomInfo = new();
    public List<RoomInfo> RoomList => _roomInfo;

    public event Action<List<RoomInfo>> OnRoomListUpdated;

    public bool TryPlayerConnect(string nickname)
    {
        try
        {
			PhotonNetwork.NickName = nickname;
			PhotonNetwork.AutomaticallySyncScene = true; 
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.ConnectToRegion($"{_regionToken}");
		}
        catch(Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
        return true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomInfo = roomList;
        OnRoomListUpdated?.Invoke(_roomInfo);
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();  
    }

    public override void OnJoinedRoom() => SceneTransition.SwitchToScene("Game");

    public override void OnLeftRoom() => SceneTransition.SwitchToScene("Rooms");

    public override void OnDisconnected(DisconnectCause cause) => PhotonNetwork.OpRemoveCompleteCacheOfPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
}

public enum RegionToken
{
    eu,
    au,
    us,
    tr,
    asia,
    cae,
    jp
}
