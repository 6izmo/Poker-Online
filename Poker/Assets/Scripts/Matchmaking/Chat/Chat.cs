using TMPro;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Chat : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI _chat;

    [PunRPC]
    public void OnSendMessage(string playerNickName, string message)
    {
        if (message == "")
            return;

        _chat.text += "\n" + playerNickName + ": " + message;
    }
}
