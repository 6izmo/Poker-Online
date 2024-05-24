using Photon.Pun;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Cards
{
    [RequireComponent(typeof(PhotonView))]
    public class CardPresenter : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        [SerializeField] private CardData _cardData;
        private AudioSource _audioSource;
		private CardModel _cardModel;
		private CardView _cardView;

		public CardModel CardModel => _cardModel;

        public void Init(CardModel model)
        {
            _cardView = GetComponent<CardView>();
			_audioSource = GetComponent<AudioSource>();
			_cardModel = model;

            _audioSource.PlayOneShot(_cardData.CardDealClip);
			PhotonNetwork.AddCallbackTarget(this);
        }

        public async UniTask Open(bool showCard = false)
        {
			_audioSource.PlayOneShot(_cardData.CardOpenClip);
			await _cardView.RotateCard(_cardModel.IsOpened, showCard);
            _cardModel.IsOpened = !_cardModel.IsOpened;
        }

        public void Showdown() => _cardView.OnShowdown(_cardModel);

        public Sprite GetOwnSprite() => CardData.GetSprite(_cardModel);

        public async void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            int operationId = (int)instantiationData[0];
            Vector2 startPosition = (Vector2)instantiationData[1];
            Vector2 destinationPosiiton;
            float angle = (float)instantiationData[3];

            if (operationId == (int)EventCode.TableCard)
            {
                destinationPosiiton = (Vector2)instantiationData[2];
                await SetPosition(startPosition, destinationPosiiton, angle);
                return;
            }

            int actorPlayerId = (int)instantiationData[2];

            if (actorPlayerId + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
                return;

            string id = photonView.InstantiationId.ToString();
            int posIndex = int.Parse(id.Substring(id.Length - 1));
            destinationPosiiton = _cardData.GetCardPosition(actorPlayerId, posIndex - 1);
            await SetPosition(startPosition, destinationPosiiton, angle);
        }

        public async UniTask SetPosition(Vector2 startPosition, Vector2 destinationPosition, float angle)
        {
            float elapsedTime = 0f;
            float animaitonRotateTime = 0.55f;
            angle -= 360;
            transform.position = startPosition;

            while (elapsedTime < animaitonRotateTime)
            {
                float zAngle = Mathf.Lerp(0, angle, elapsedTime / animaitonRotateTime);
                transform.rotation = Quaternion.Euler(0, 0, zAngle);
                transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime / animaitonRotateTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = destinationPosition;
            await UniTask.CompletedTask;
        }

        public void OnDestroy() => PhotonNetwork.RemoveCallbackTarget(this);
    }
}
