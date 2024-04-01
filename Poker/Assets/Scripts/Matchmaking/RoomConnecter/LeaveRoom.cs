using Photon.Pun;
using UnityEngine.EventSystems;

public class LeaveRoom : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) => PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom() => SceneTransition.SwitchToScene("Rooms");
}
