using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerList
{
    public class PlayerListView : MonoBehaviour
    {
        [Header("Ready Button")]
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _readyConditionText;
        [SerializeField] private Image _outline;
        [Space]
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Transform _contentView;
        [SerializeField] private GameObject _listPanel;

        private PlayerListPresenter _presenter;
        private bool _readyCondition = false;

		public event Action OnLeaved;

		public void Init(PlayerListPresenter presenter) => _presenter = presenter;

        private void Awake() 
        {
            _readyButton.Add(() => OnReadyClick());
			_leaveButton.Add(() => OnLeaved?.Invoke());

            _readyConditionText.color = Color.red;
            _outline.color = Color.red;
        }

        public PlayerInfoView ShowPlayerToPlayerPanel(PlayerInfoView playerItemPrefab)
        {
            PlayerInfoView playerItem = Instantiate(playerItemPrefab, _contentView);
            return playerItem; 
        }

        public void RemovePlayerFromPlayerList(PlayerInfoView playerItem) => Destroy(playerItem.gameObject);

        public void HideList()
        {
            _readyButton.Deactivate();
            _listPanel.Deactivate();
        }

        private void OnReadyClick()
        {
            _readyCondition = !_readyCondition;
            Color color = _readyCondition ? Color.green : Color.red;
            _readyConditionText.color = color;
            _outline.color = color;
            _presenter.UpdateReadyCondition(_readyCondition);
        }
    }
}
