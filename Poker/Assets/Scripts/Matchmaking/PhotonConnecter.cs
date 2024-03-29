using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class PhotonConnecter : MonoBehaviourPunCallbacks
{
    [SerializeField] private RegionToken _regionToken;
    [Space]
    [SerializeField] private string _gameVersion = "0.0.1";

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.NickName = "Players - " + UnityEngine.Random.Range(0, 100);

        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion($"{_regionToken}");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.NickName + " - вы подключились к серверу");
        print(PhotonNetwork.CloudRegion);

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();  
    }

    public override void OnJoinedRoom() => PhotonNetwork.LoadLevel("Game");

    public override void OnDisconnected(DisconnectCause cause) => print("Вы отключились");
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
