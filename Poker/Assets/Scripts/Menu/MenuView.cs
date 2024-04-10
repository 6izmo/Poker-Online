using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuView : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _infoButton;
        [Space]
        [SerializeField] private TMP_InputField _inputField;
        [Space]
        [SerializeField] private Text _title;

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputedName;
            
        public void Init(bool isConnected)
        {
            _playButton.onClick.AddListener(delegate { OnPlayButtonClicked?.Invoke(); _title.Deactivate(); });
            _okButton.onClick.AddListener(delegate { SwitchPanel(); OnInputedName?.Invoke(_inputField.text); });

            if (isConnected)
                SwitchPanel();
		}

        private void SwitchPanel()
        {
            _inputField.Deactivate();
            _okButton.Deactivate();
            _playButton.Activate();
			_settingsButton.Activate();
            _infoButton.Activate();
        }
    }
}
