using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    [Header("Outline")]
    [SerializeField] private Image _outline;
    [SerializeField] private Color _defaultColor;
    [Space]
    [SerializeField] private TextMeshProUGUI _roomName;
    [SerializeField] private TextMeshProUGUI _playersCount;

    public void OnPointerClick(PointerEventData eventData) => PhotonNetwork.JoinRoom(_roomName.text);

    public void SetRoomInfo(RoomInfo info)
    {
        _outline.color = info.IsOpen ? _defaultColor : Color.red;
        _roomName.text = info.Name;
        _playersCount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }
}
