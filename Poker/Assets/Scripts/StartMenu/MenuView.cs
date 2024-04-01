using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _musicButton;
        [SerializeField] private Button _infoButton;
        [Space]
        [SerializeField] private TMP_InputField _inputField;
        [Space]
        [SerializeField] private Text _title;

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputedName;

        public void Init()
        {
            _playButton.onClick.AddListener(delegate { OnPlayButtonClicked?.Invoke(); _title.Deactivate(); });
            _okButton.onClick.AddListener(delegate { OnInputedName?.Invoke(_inputField.text); SwitchPanel(); });
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
