using UnityEngine;

namespace PokerMatch
{
    public class PokerMatchView : MonoBehaviour
    {
        [SerializeField] private GameObject _cardDeck;

        [Header("UI")]
        [SerializeField] private RectTransform _allPlayersParent;

        public void CardDeckActivate() => _cardDeck.SetActive(true);

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
