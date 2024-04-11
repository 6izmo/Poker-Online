using Players;
using PokerMatch;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Cards
{
    public class CardSpawner : IOnEventCallback
    {
        private PokerMatchModel _pokerModel;

        public CardSpawner(PokerMatchModel pokerModel)
        {
            _pokerModel = pokerModel;
            PhotonNetwork.AddCallbackTarget(this);
        }

        public async void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != (int)EventCode.Dealing)
                return;

            object[] datas = (object[])photonEvent.CustomData;
            Player player = (Player)datas[1];
            string prefabName = (string)datas[3];
            var playerId = player.ActorNumber - 1;
            int cardIndex = (int)datas[4];

            Vector2 localPositon = _pokerModel.CardData.GetCardPosition(playerId, cardIndex);
            float rotation = new System.Random().Next(-25, 25);

            if (player != PhotonNetwork.LocalPlayer)
                return;

            object[] dataPos = new object[] { 1, _pokerModel.CardData.CardDeckPosition, playerId, rotation };
            CardModel cardModel = (CardModel)datas[2];
            GameObject cardObject = PhotonNetwork.Instantiate(prefabName, localPositon, Quaternion.identity, 0, dataPos);

            CardPresenter cardPresenter = cardObject.GetComponent<CardPresenter>();
            cardPresenter.Init(cardModel);
            await cardPresenter.SetPosition(_pokerModel.CardData.CardDeckPosition, localPositon, 0);

            PlayerModel playerModel = _pokerModel.GetPlayerModel(PhotonNetwork.LocalPlayer);
            playerModel.AddCard(cardPresenter);
        }

        public void RemoveCallback() => PhotonNetwork.RemoveCallbackTarget(this);
    }
}
