using TMPro;
using UnityEngine;
using System.Threading.Tasks;

namespace Bank
{
    public class BankView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bank;

        public void ActivateBank(bool activate) => _bank.gameObject.SetActive(activate);

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
                await Task.Yield();
            }
            _bank.text = $"BANK:{value}$";
        }
    }
}
