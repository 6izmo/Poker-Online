using TMPro;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class CreateRoom : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    [SerializeField] private TMP_InputField _inputField;

    public void OnPointerClick(PointerEventData eventData) => CreateNewRoom();

    private void CreateNewRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        RoomOptions roomOptions = new()
        {
            MaxPlayers = 4,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(_inputField.text, roomOptions, TypedLobby.Default);
    }
}
