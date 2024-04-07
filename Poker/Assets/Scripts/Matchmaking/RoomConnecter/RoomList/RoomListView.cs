using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomList
{
    public class RoomListView : MonoBehaviour
    {
        [SerializeField] private Transform _contentView;
        [Space]
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        [Space]
        [SerializeField] private TMP_InputField _inputField;

        public event Action<string> OnRoomCreated;
        public event Action<string> OnRoomJoined;
        public event Action OnExit;

        public void Init()
        {
            _leaveButton.onClick.AddListener(delegate { OnExit?.Invoke(); });
            _createButton.onClick.AddListener(delegate { OnRoomCreated?.Invoke(_inputField.text); });
            _joinButton.onClick.AddListener(delegate { OnRoomJoined?.Invoke(_inputField.text); });
        }

        public RoomItem ShowRoom(RoomItem prefab) => Instantiate(prefab, _contentView);
    }
}
