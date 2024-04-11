using UnityEngine;
using UnityEngine.UI;

namespace PokerMatch
{
    public class PokerMatchView : MonoBehaviour
    {
        [SerializeField] private GameObject _cardDeck;

        [Header("UI")]
        [SerializeField] private RectTransform _allPlayersParent;
        [Space]
        [SerializeField] private Button _chatButton;
        [SerializeField] private Image _cross;
		[SerializeField] private RectTransform _chat;

		private void Awake()
		{
            _cross.Deactivate();
			_chatButton.onClick.AddListener(() => SetActiveChat());
		}

        private void SetActiveChat()
        {
            bool active = _cross.gameObject.activeSelf ? true : false;
            _cross.gameObject.SetActive(!active);
            _chat.gameObject.SetActive(active);
		}

		public void CardDeckActivate() => _cardDeck.SetActive(true);

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
