using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class PhotonConnecter : MonoBehaviourPunCallbacks
{
    [SerializeField] private RegionToken _regionToken;
    [Space]
    [SerializeField] private string _gameVersion = "0.0.1";

    private void Awake() => DontDestroyOnLoad(this);

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

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Вы отключились - " + cause);
        PhotonNetwork.OpRemoveCompleteCacheOfPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
    }
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
