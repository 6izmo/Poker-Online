using UnityEngine;

namespace PokerMatch
{
    public class PokerMatchView : MonoBehaviour
    {
        [SerializeField] private GameObject _cardDeck;

        [Header("UI")]
        [SerializeField] private RectTransform _allPlayersParent;
        [Space]
        [Header("Event Text")]
        [SerializeField] private Vector3 _textScale;
        [SerializeField, Range (0f, 2f)] private float _scaleDuration;
        [SerializeField, Range(0f, 3f)] private float _textDuration;

        public void CardDeckActivate() => _cardDeck.SetActive(true);

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
