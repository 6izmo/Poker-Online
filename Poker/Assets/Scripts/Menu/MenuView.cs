using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Menu
{
    public class MenuView : MonoBehaviour
    {
		[Header("Main Menu")]
		[SerializeField] private Text _title;
        [Space]
		[SerializeField] private Button _settingsButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _infoButton;

        [Header("Start Panel")]
        [SerializeField] private RectTransform _startPanel;
		[SerializeField] private Button _okButton;
        [SerializeField] private TMP_InputField _inputField;

        [Header("ConnectionPanel")]
        [SerializeField] private RectTransform _connectionPanel;   
        [SerializeField] private Button _repeatButton;
        [SerializeField] private Image _spinner;

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputedName;
            
        private void Start()
        {
            _playButton.onClick.AddListener(() => { OnPlayButtonClicked?.Invoke(); _title.Deactivate(); });
            _okButton.onClick.AddListener(() => OnInputedName?.Invoke(_inputField.text));
            _repeatButton.onClick.AddListener(() => { SwitchToConnectionPanel(false); _repeatButton.Deactivate(); });

            if (PhotonNetwork.IsConnected)
                ActivateMainMenu();
		}

        public void ActivateRepeatButton()
        {
            _repeatButton.Activate();
            _spinner.Deactivate();
		}

		public void SwitchToConnectionPanel(bool active)
        {
            _startPanel.gameObject.SetActive(!active);
			_connectionPanel.gameObject.SetActive(active);
		}

        public void ActivateMainMenu()
        {
            _connectionPanel.Deactivate();
            _startPanel.Deactivate();
			_playButton.Activate();
			_settingsButton.Activate();
            _infoButton.Activate();
        }
    }
}
