using System;
using UnityEngine;

namespace Bank
{
    public class BankModel
    {
        public static event Action<int> OnCurrentRateChanged;

        public ObservableVariable<int> AmoutMoney { get; private set; }

        public static int CurrentRate { get; private set; }

        public static int BigBlind { get; private set; }

        public static int SmallBlind { get; private set; }

        public int LastAmount { get; set; }

        public BankModel(int smallBlind, int bigBlind)
        {   
            AmoutMoney = new();

            SmallBlind = smallBlind;
            BigBlind = bigBlind;
        }

        public void ChangeRate(int rate)
        {
            Debug.Log($" change rate - {rate}");
            CurrentRate = rate;
            OnCurrentRateChanged?.Invoke(CurrentRate);
        }
    }
}
