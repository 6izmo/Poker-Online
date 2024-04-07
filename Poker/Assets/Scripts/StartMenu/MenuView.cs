using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuView : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private Button _musicButton;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _muted;
        [SerializeField] private Sprite _unmuted;
        [Space]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _infoButton;
        [Space]
        [SerializeField] private TMP_InputField _inputField;
        [Space]
        [SerializeField] private Text _title;

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputedName;
        public event Action<bool> OnMutedMusic;

        private bool _isMusicMuted = false;

        public void Init()
        {
            _playButton.onClick.AddListener(delegate { OnPlayButtonClicked?.Invoke(); _title.Deactivate(); });
            _okButton.onClick.AddListener(delegate { OnInputedName?.Invoke(_inputField.text); SwitchPanel(); });
            _musicButton.onClick.AddListener(delegate { MuteMusic(); });
        }

        private void MuteMusic()
        {
            _isMusicMuted = !_isMusicMuted;
            _icon.sprite = _isMusicMuted ? _muted : _unmuted;
            OnMutedMusic?.Invoke(_isMusicMuted);
        }

        private void SwitchPanel()
        {
            _inputField.Deactivate();
            _okButton.Deactivate();
            _playButton.Activate();
            _musicButton.Activate();
            _infoButton.Activate();
        }
    }
}
