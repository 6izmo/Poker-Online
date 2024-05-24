using TMPro;
using System;
using DG.Tweening;
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
        [SerializeField] private TextMeshProUGUI _placeHolder;

        public event Action<string> OnRoomCreated;
        public event Action<string> OnRoomJoined;
        public event Action OnRoomRandomJoined;
        public event Action OnExit;

        private Tween _tweenShake;

        private bool _isJoinedByName = false;

        public void Init()
        {
            _leaveButton.Add(() => OnExit?.Invoke());
            _sendButton.Add(() => OnSendButton());
            _joinRandomButton.Add(() => OnRoomRandomJoined?.Invoke());
		}

        private void OnSendButton()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                _placeHolder.color = Color.red;   
                if(_tweenShake != null && _tweenShake.IsPlaying()) 
                    _tweenShake.Kill(); 
                _tweenShake = _placeHolder.rectTransform.DOShakePosition(1f);
                return;
            }

            if (_isJoinedByName)
                OnRoomJoined?.Invoke(_inputField.text);
            else
                OnRoomCreated?.Invoke(_inputField.text);
		}

        public void OnJoinedByName(bool value) => _isJoinedByName = value;

		public RoomItem ShowRoom(RoomItem prefab) => Instantiate(prefab, _contentView);
    }
}
