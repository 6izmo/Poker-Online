using UnityEngine;
using UnityEngine.UI;

namespace PokerMatch
{
    public class MatchView : MonoBehaviour
    {
        [SerializeField] private GameObject _cardDeck;

        [Header("UI")]
        [SerializeField] private RectTransform _allPlayersParent;
        [Space]
        [SerializeField] private Button _chatButton;
        [SerializeField] private Image _cross;
		[SerializeField] private RectTransform _chat;
        [SerializeField] private Text _congratulations;

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

        public void OnEndMatch(string winnerName)
        {
            _cardDeck.Deactivate();
            _congratulations.Activate();
            _congratulations.text = "Game Over " + '\n' + winnerName + " Won";
		}

		public void SetActiveCardDeck(bool active) => _cardDeck.SetActive(active);

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
