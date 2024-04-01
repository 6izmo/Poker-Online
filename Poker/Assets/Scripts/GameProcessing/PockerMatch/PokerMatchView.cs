using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

namespace PokerMatch
{
    public class PokerMatchView : MonoBehaviour
    {
        [SerializeField] private GameObject _cardDeck;

        [Header("UI")]
        [SerializeField] private RectTransform _allPlayersParent;
        [Space]
        [Header("Event Text")]
        [SerializeField] private TextMeshProUGUI _eventText;
        [SerializeField] private Vector3 _textScale;
        [SerializeField, Range (0f, 2f)] private float _scaleDuration;
        [SerializeField, Range(0f, 3f)] private float _textDuration;

        public void CardDeckActivate() => _cardDeck.SetActive(true);

        public async void DisplayMessage(string message, ColorModel colorModel)
        {
            _eventText.gameObject.SetActive(true);
            _eventText.color = colorModel.Color;
            _eventText.transform.DOScale(_textScale, _scaleDuration);
            _eventText.text = message;
            await Task.Delay((int)(_textDuration * 1000));
            DOTween.Sequence()
                .Append(_eventText.transform.DOScale(Vector3.zero, _scaleDuration))
                .AppendCallback(() => { _eventText.Deactivate(); });
        }

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
