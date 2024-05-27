using TMPro;
using Utilities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Chat : SingletonPun<Chat>
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI _chat;

    public void SendMessage()
    {
        if (_inputField.text.Length > 0)
        {
            photonView.RPC("SendMessagePun", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, _inputField.text);
            _inputField.text = string.Empty; 
        }    
    }

    [PunRPC]
    public void SendMessagePun(string playerNickName, string message)
    {
        if (message == "")
            return;

        _chat.text += "\n" + playerNickName + ": " + message;
    }
}
