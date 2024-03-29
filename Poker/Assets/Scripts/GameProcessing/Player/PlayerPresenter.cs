using Bank;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Threading.Tasks;

namespace Players
{
    using Move = PlayerModel.PlayerMove;
    public class PlayerPresenter
    {
        private PlayerView _playerView;
        private PlayerModel _playerModel;
        
        public PlayerPresenter(PlayerModel model, PlayerView view)
        {
            _playerModel = model;
            _playerView = view;

            BankModel.OnCurrentRateChanged += (context) => SetRaiseValue(context - _playerModel.Rate.Value + BankModel.BigBlind);
            BankModel.OnCurrentRateChanged += _playerView.UpdateCallButton;

            _playerModel.RaiseSum.OnChanged += _playerView.UpdateRaiseButton;
            _playerModel.Money.OnChanged += _playerView.UpdateMoneyText;
            _playerModel.Folded.OnChanged += _playerView.HideChechCards;

            _playerView.OnRaiseChanged += SetRaiseValue;
            _playerView.OnTurnedOver += TurnOverCards;
            _playerView.OnMoved += OnMove;
        }
           
        private void SetRaiseValue(int value)
        {
            if (value > 0 && value < _playerModel.Money.Value && value >= (BankModel.CurrentRate - _playerModel.Rate.Value) + BankModel.BigBlind)
                _playerModel.RaiseSum.Value = value;
        }

        public void TurnOverCards()
        {
            for (int i = 0; i < _playerModel.Cards.Count; i++)
                _ = _playerModel.Cards[i].Open();
        }

        public void OnMove(Move move)
        {
            if (_playerModel.CurrentState != PlayerModel.PlayerState.Move)
                return;

            object[] content = new object[] { PhotonNetwork.LocalPlayer, move, _playerModel };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)EventCode.Move, content, eventOptions, SendOptions.SendUnreliable);
        }
    }
}
