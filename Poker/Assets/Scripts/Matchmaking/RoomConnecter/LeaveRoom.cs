using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LeaveRoom : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) => PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom() => SceneManager.LoadScene("Menu");
}
