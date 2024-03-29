using Players;
using UnityEngine;
using System.Threading.Tasks;

namespace Bank
{
    public class BankPresenter
    {
        private BankModel _bankModel;
        private BankView _bankView;

        public BankPresenter(BankModel model, BankView view)
        {
            _bankModel = model;
            _bankView = view;

            _bankModel.AmoutMoney.OnChanged += (context) => _bankView.UpdateBank(_bankModel.LastAmount, context);
        }

        public void AddMoney(int money)
        {
            _bankModel.LastAmount = _bankModel.AmoutMoney.Value;
            _bankModel.AmoutMoney.Value += money;
        }

        public void ActivateBank() => _bankView.ActivateBank(true);

        public void ChangeRate(int rate) => _bankModel.ChangeRate(rate);

        public Task GiveAwayTheWinnings(PlayerModel playerModel)
        {
            playerModel.Money.Value += _bankModel.AmoutMoney.Value;
            _bankModel.AmoutMoney.Value = 0;
            Debug.Log($"Complete");
            return Task.CompletedTask;
        }
    }
}
