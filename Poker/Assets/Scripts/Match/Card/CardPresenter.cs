using System;
using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;

namespace Cards
{
    [RequireComponent(typeof(PhotonView))]
    public class CardPresenter : MonoBehaviourPun, IPunInstantiateMagicCallback, IDisposable
    {
        [SerializeField] private CardData _cardData;
        private CardModel _cardModel;
        private CardView _cardView;

        public CardModel CardModel => _cardModel;

        public void Init(CardModel model)
        {
            _cardView = GetComponent<CardView>();
            _cardModel = model;

            PhotonNetwork.AddCallbackTarget(this);
        }

        public async Task Open(bool showCard = false)
        {
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

        public async Task SetPosition(Vector2 startPosition, Vector2 destinationPosition, float angle)
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
                await Task.Yield();
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = destinationPosition;
            await Task.CompletedTask;
        }

        public void Dispose() => PhotonNetwork.RemoveCallbackTarget(this);
    }
}