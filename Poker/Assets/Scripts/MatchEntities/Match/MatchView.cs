using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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

        [Header("Button Blinds")]
        [SerializeField, Range(0.1f, 2f)] private float _translateTime;
        [SerializeField] private GameObject _smallBlinds;
        [SerializeField] private GameObject _bigBlinds;

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
            SetActiveView(false);
            _congratulations.Activate();
            _congratulations.text = winnerName.Equals(PhotonNetwork.LocalPlayer.NickName) ? "You Win!" : "Game Over " + '\n' + winnerName + " Won";
		}

        public async UniTask SetButtonBlind(Vector2 smallPosition, Vector2 bigPosition)
        {
            Vector2 smallStart = _smallBlinds.transform.position;
            Vector2 bigStart = _bigBlinds.transform.position;
            float elapsedTime = 0;
            while (elapsedTime < _translateTime)  
            {
                _smallBlinds.transform.position = Vector3.Lerp(smallStart, smallPosition, elapsedTime / _translateTime);
                _bigBlinds.transform.position = Vector3.Lerp(bigStart, bigPosition, elapsedTime / _translateTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();   
            }
            _smallBlinds.transform.position = smallPosition;  
            _bigBlinds.transform.position = bigPosition;
            await UniTask.CompletedTask;  
        }

        public void SetActiveView(bool active)
        {
            _cardDeck.gameObject.SetActive(active);
            _smallBlinds.gameObject.SetActive(active);
            _bigBlinds.gameObject.SetActive(active);
        }

        public void ShowPlayersInPlaces(PlayerInfoView playerItem, PlayerItemPosition playerItemPosition)
        {
            playerItem.HideReadyIcon();
            playerItem.SetPosition(playerItemPosition, _allPlayersParent);
        }
    }
}
