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
        [SerializeField] private Button _sendButton;
		[SerializeField] private Button _joinRandomButton;
		[Space]
        [SerializeField] private TMP_InputField _inputField;

        public event Action<string> OnRoomCreated;
        public event Action<string> OnRoomJoined;
        public event Action OnRoomRandomJoined;

        public event Action OnExit;

        private bool _isJoinedByName = false;

        public void Init()
        {
            _leaveButton.onClick.AddListener(() => OnExit?.Invoke());
            _sendButton.onClick.AddListener(() => OnSendButton());
            _joinRandomButton.onClick.AddListener(() => OnRoomRandomJoined?.Invoke());
		}

        private void OnSendButton()
        {
            if (_isJoinedByName)
                OnRoomJoined?.Invoke(_inputField.text);
            else
                OnRoomCreated?.Invoke(_inputField.text);
		}

        public void OnJoinedByName(bool value) => _isJoinedByName = value;

		public RoomItem ShowRoom(RoomItem prefab) => Instantiate(prefab, _contentView);
    }
}
