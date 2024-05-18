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
        [SerializeField] private Button _exitButton;

        [Header("Start Panel")]
        [SerializeField] private RectTransform _startPanel;
		[SerializeField] private Button _okButton;
        [SerializeField] private TMP_InputField _inputField;

        [Header("ConnectionPanel")]
        [SerializeField] private RectTransform _connectionPanel;   
        [SerializeField] private Button _repeatButton;

        public event Action OnPlayButtonClicked;
        public event Action<string> OnInputedName;
            
        private void Start()
        {
            _playButton.onClick.AddListener(() => { OnPlayButtonClicked?.Invoke(); _title.Deactivate(); });
            _okButton.onClick.AddListener(() => OnInputedName?.Invoke(_inputField.text));
            _repeatButton.onClick.AddListener(() => { SwitchToConnectionPanel(false); _repeatButton.Deactivate(); });
            _exitButton.onClick.AddListener(() => Application.Quit());

			if (PhotonNetwork.IsConnected)  
                ActivateMainMenu();
		}

        public void ActivateRepeatButton() => _repeatButton.Activate();

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
            _exitButton.Activate();
			_settingsButton.Activate();
            _infoButton.Activate();
        }
    }
}
