using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerList
{
    public class PlayerListView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Transform _contentView;
        [SerializeField] private GameObject _listPanel;
        [Space]
        [SerializeField] private TextMeshProUGUI _readyConditionText;

        private PlayerListPresenter _presenter;
        private Image _image;
        private bool _readyCondition = false;

        public void Init(PlayerListPresenter presenter) => _presenter = presenter;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _readyButton.onClick.AddListener(() => OnReadyClick());

            _readyConditionText.text = "Unready";
            _readyConditionText.color = Color.red;
        }

        public PlayerInfoView ShowPlayerToPlayerPanel(PlayerInfoView playerItemPrefab)
        {
            PlayerInfoView playerItem = Instantiate(playerItemPrefab, _contentView);
            return playerItem;
        }

        public void RemovePlayerFromPlayerList(PlayerInfoView playerItem) => Destroy(playerItem.gameObject);

        public void HideList()
        {
            _image.enabled = false;
            _readyButton.Deactivate();
            _listPanel.Deactivate();
        }

        private void OnReadyClick()
        {
            _readyCondition = !_readyCondition;

            _readyConditionText.text = _readyCondition ? "Ready" : "Unready";
            _readyConditionText.color = _readyCondition ? Color.green : Color.red;

            _presenter.UpdateReadyCondition(_readyCondition);
        }
    }
}
