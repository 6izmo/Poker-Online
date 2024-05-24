using System;
using Photon.Pun;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Cards
{
    [RequireComponent(typeof(PhotonView))]
    public class CardView : MonoBehaviourPunCallbacks
    {
        private SpriteRenderer _spriteRenderer;
        private CardPresenter _cardPresenter;
        private Sprite _backSprite;

        private bool _isTurning = false;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _cardPresenter = GetComponent<CardPresenter>();
            _backSprite = _spriteRenderer.sprite;
        }

        public async UniTask RotateCard(bool isOpened, bool showCard = false)
        {
            if (_isTurning)
                return;

            _isTurning = true;
            float rotateTime = 0.25f;

            Vector3 startRotation = transform.localEulerAngles;
            Vector3 neededRotation = startRotation;
            Vector3 scale = transform.localScale;
            neededRotation.y = isOpened ? 270f :  -90f;

            await Rotate(rotateTime, startRotation, neededRotation);

            _spriteRenderer.sprite = isOpened ? _backSprite : _cardPresenter.GetOwnSprite();
            scale.x = isOpened ? Math.Abs(scale.x) : -scale.x;

            if (showCard)
                OnShowdown(_cardPresenter.CardModel);

            startRotation = neededRotation;
            neededRotation.y = isOpened ? 360f : -180f;
            transform.localScale = scale;

            await Rotate(rotateTime, startRotation, neededRotation);
            _isTurning = false;
        }

        private async UniTask Rotate(float rotateTime, Vector3 startRotation, Vector3 neededRotation)    
        {
            float elapsedTime = 0f;
            while (elapsedTime < rotateTime)
            {
                transform.localEulerAngles = Vector3.Lerp(startRotation, neededRotation, elapsedTime / rotateTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }
            transform.localEulerAngles = neededRotation;
            await UniTask.CompletedTask;
        }

        public void OnShowdown(CardModel cardModel) => photonView.RPC("ShowCard", RpcTarget.All, cardModel);

        [PunRPC]
        public void ShowCard(CardModel cardModel) => _spriteRenderer.sprite = CardData.GetSprite(cardModel);
    }
}

