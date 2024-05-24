using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Bank
{
    public class BankView : MonoBehaviour  
    {
        [SerializeField] private TextMeshProUGUI _bank;

        public void Activate() => _bank.Activate();

        public async void UpdateBank(int lastValue, int value)
        {
            float animationTime = 1f;
            float elapsedTime = 0f;
            int currentValue;
            while (elapsedTime < animationTime)
            {
                currentValue = (int)Mathf.Lerp(lastValue, value, elapsedTime / animationTime);
                elapsedTime += Time.deltaTime;
                _bank.text = $"BANK:{currentValue}$";
                await UniTask.Yield();
            }
            _bank.text = $"BANK:{value}$";
        }
    }
}
