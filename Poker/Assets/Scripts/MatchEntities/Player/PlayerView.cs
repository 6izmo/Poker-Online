using Bank;
using TMPro;
using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Players
{
    using Move = PlayerModel.PlayerMove;

    public class PlayerView : MonoBehaviourPun
    {
        [SerializeField] private RectTransform _commandPanel;
        private PlayerModel _playerModel;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _callText;
        [SerializeField] private TextMeshProUGUI _raiseText;

        [Header("Buttons")]
        [SerializeField] private Button _checkButton;
        [SerializeField] private Button _allInButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _raiseButton;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _callButton;
        [SerializeField] private Button _foldButton;

        public event Action OnTurnedOver;
        public event Action<Move> OnMoved;
        public event Action<int> OnRaiseChanged;

        public void Init(PlayerModel playerModel)
        {
            _playerModel = playerModel;

            _moneyText.Activate();
            _commandPanel.Activate();
            _checkButton.Deactivate();

            _moneyText.text = $"{_playerModel.Money}$";

            _checkButton.Add(() => OnTurnedOver?.Invoke());
            _callButton.Add(()  => OnMoved?.Invoke(Move.Call));
            _raiseButton.Add(() => OnMoved?.Invoke(Move.Raise) );
            _allInButton.Add(() => OnMoved?.Invoke(Move.AllIn));
            _foldButton.Add(()  => OnMoved?.Invoke(Move.Fold));
            _plusButton.Add(()  => OnRaiseChanged(_playerModel.RaiseSum.Value + BankModel.BigBlind)); 
            _minusButton.Add(() => OnRaiseChanged(_playerModel.RaiseSum.Value - BankModel.BigBlind));
        }

        public void SetActiveCardButton(bool active) => _checkButton.gameObject.SetActive(active);

        public void SetActiveCommandPanel(bool folded) => _commandPanel.gameObject.SetActive(!folded);

        public void UpdateRaiseButton(int sum) => _raiseText.text = $"RAISE({sum})";

        public void ChangeCallToCheck() => _callText.text = "CHECK";

		public async void UpdateMoneyText(int money)
        {
            float animationTime = 0.75f;
            float elapsedTime = 0f;
            int value;
            while (elapsedTime < animationTime)
            {
                value = (int)Mathf.Lerp(_playerModel.LastAmountMoney, money, elapsedTime / animationTime);
                elapsedTime += Time.deltaTime;
                _moneyText.text = $"{value}$";
                await Task.Yield();
            }
            _moneyText.text = $"{money}$";
            _playerModel.LastAmountMoney = money;
        }

        public void UpdateCallButton(int currentRate)
        {
            int callSum = currentRate <= _playerModel.Money.Value ? currentRate - _playerModel.Rate.Value : _playerModel.Money.Value;
			_callText.text = callSum <= 0 ? "CHECK": "CALL " + $"({callSum})";
        }
    }
}
