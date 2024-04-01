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

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputName;

        public void Init()
        {
            _playButton.onClick.AddListener(delegate { OnPlayButtonClicked?.Invoke(); });
            _okButton.onClick.AddListener(delegate { OnInputName?.Invoke(_inputField.text); ActivateMainUI(); });
        }

        private void ActivateMainUI()
        {
            _inputField.Deactivate();
            _okButton.Deactivate();
            _playButton.Activate();
            _musicButton.Activate();
            _infoButton.Activate();
        }
    }
}
