using TMPro;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private TextMeshProUGUI _playersCount;

    public RoomInfo RoomInfo { get; private set; }

    public void OnPointerClick(PointerEventData eventData) => PhotonNetwork.JoinRoom(_roomName.text);  

    public void SetRoomInfo(RoomInfo info)
    {
        RoomInfo = info;

        _roomName.text = info.Name;
        _playersCount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }
}
