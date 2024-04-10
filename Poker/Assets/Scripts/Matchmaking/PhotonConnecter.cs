using Utilities;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class PhotonConnecter : PersistentSingletonPun<PhotonConnecter> 
{
    [SerializeField] private RegionToken _regionToken;
    [Space]
    [SerializeField] private string _gameVersion = "0.0.1";

    public void PlayerConect(string nickname)
    {
        if (PhotonNetwork.IsConnected)
            return;

        if (nickname == null)
            nickname = $"Player_{new System.Random().Next(0, 100)}";

        PhotonNetwork.NickName = nickname;

        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion($"{_regionToken}");
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
