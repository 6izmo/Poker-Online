using Players;

namespace Bank
{
    public class BankPresenter : Presenter
    {
        private BankModel _bankModel;
        private BankView _bankView;

        public BankPresenter(BankModel model, BankView view) : base()
        {
            _bankModel = model;
            _bankView = view;

            _bankView.Activate();
            _bankModel.AmoutMoney.OnChanged += UpdateBank;
		}

        public void AddMoney(int money)
        {
            _bankModel.LastAmount = _bankModel.AmoutMoney.Value;
            _bankModel.AmoutMoney.Value += money;
        }

        private void UpdateBank(int money) => _bankView.UpdateBank(_bankModel.LastAmount, money);

        public void ChangeRate(int rate) => _bankModel.ChangeRate(rate);

        public void ReturnBet(PlayerModel player)
        {
            player.Money.Value += player.AllBetsInRound;
            _bankModel.ClearBank(); 
        }

        public void TransferMoneyToPlayer(PlayerModel playerModel)
        {
            playerModel.Money.Value += _bankModel.AmoutMoney.Value;
            _bankModel.ClearBank();
		}

        public override void Dispose() => _bankModel.AmoutMoney.OnChanged -= UpdateBank;
    }
}
