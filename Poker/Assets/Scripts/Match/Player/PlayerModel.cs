using Bank;
using Cards;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Players
{
    public class PlayerModel
    {
        public enum PlayerMove
        {
            Fold,
            Check,
            Call,
            Raise,
            AllIn
        }

        public enum PlayerState
        {
            Waiting,
            Move
        }

        private List<CardPresenter> _cardsInfo = new();

        public List<CardPresenter> Cards => _cardsInfo;

        public ObservableVariable<int> Money { get; private set; }

        public ObservableVariable<int> Rate { get; private set; }

        public ObservableVariable<bool> Folded { get; private set; }

        public ObservableVariable<int> RaiseSum { get; private set; }

        public event Action<bool> OnGotCards;

        public int LastAmountMoney { get; set; }

        public PlayerState CurrentState { get; set; }

        public PlayerModel(int startMoney)
        {
            Rate = new();
            Folded = new(false);
            Money = new(startMoney);
            LastAmountMoney = Money.Value;
            CurrentState = PlayerState.Waiting;
            RaiseSum = new(BankModel.BigBlind);
        }

        public void AddCard(CardPresenter presenter)
        {
            if(!_cardsInfo.Contains(presenter))
            {
                _cardsInfo.Add(presenter);
                if (_cardsInfo.Count == 2)
                    OnGotCards?.Invoke(true);
            }
        }

        public void Raises(int value)
        {
            Money.Value -= value;
            Rate.Value += value;
        }

        public void ResetModel()
        {
            Folded.Value = false;
            _cardsInfo.Clear();
        }

        public static byte[] Serialize(object customType)
        {
            PlayerModel myType = (PlayerModel)customType;
            int folded = myType.Folded.Value ? 1 : 0;
            int[] data = new int[] { myType.Money.Value, myType.Rate.Value, myType.RaiseSum.Value, folded };
            Span<byte> bytes = MemoryMarshal.Cast<int, byte>(data);
            return bytes.ToArray();
        }

        public static object Deserialize(byte[] bytes)
        {
            Span<int> ints = MemoryMarshal.Cast<byte, int>(bytes);
            PlayerModel model = new PlayerModel(ints[0]);
            model.Rate = new(ints[1]);
            model.RaiseSum = new(ints[2]);
            model.Folded = ints[3] == 1 ? new(true) : new(false);
            return model;
        }
    }
}
