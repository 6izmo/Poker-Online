using System;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
	public class SettingsView : MonoBehaviour
	{
		[SerializeField] private Button _close;

		[Header("Music")]
		[SerializeField] private Button _plusMusic;
		[SerializeField] private Button _minusMusic;
		[Space]
		[SerializeField] private Image _musicIcon;
		[SerializeField] private Sprite _musicOff;
		[SerializeField] private Sprite _musicOn;
		[SerializeField] private Image _musicProgressbar;

		[Header("Effects")]
		[SerializeField] private Button _plusEffects;
		[SerializeField] private Button _minusEffects;
		[Space]
		[SerializeField] private Image _effectsIcon;
		[SerializeField] private Sprite _effectsOff;
		[SerializeField] private Sprite _effectsOn;
		[SerializeField] private Image _effectsProgressbar;

		private const float _progressStep = 0.2f;
		public event Action<float> OnMusicChanged;
		public event Action<float> OnEffectsChanged;
		public event Action<float, float> OnClosed;

		public void Init(SettingsModel settings)
		{
			_close.onClick.AddListener(() => OnClosed?.Invoke(_musicProgressbar.fillAmount, _effectsProgressbar.fillAmount));

			_plusMusic.onClick.AddListener(() => SetMusicProgress(_musicProgressbar.fillAmount + _progressStep));
			_minusMusic.onClick.AddListener(() => SetMusicProgress(_musicProgressbar.fillAmount - _progressStep));
			_plusEffects.onClick.AddListener(() => SetEffectsProgress(_effectsProgressbar.fillAmount + _progressStep));
			_minusEffects.onClick.AddListener(() => SetEffectsProgress(_effectsProgressbar.fillAmount - _progressStep));

			float musicValue = settings == null ? 1f : settings.Music;
			float effectsValue = settings == null ? 1f : settings.Effects;

			SetMusicProgress(musicValue);
			SetEffectsProgress(effectsValue);
		}

		public void SetMusicProgress(float value = 1f)
		{
			_musicProgressbar.fillAmount = value;
			switch (_musicProgressbar.fillAmount)
			{
				case < .2f:
					_musicIcon.sprite = _musicOff;
					break;
				case < .4f:
					_musicIcon.sprite = _musicOn;
					_musicProgressbar.color = Color.red;
					break;
				case < .8f:
					_musicProgressbar.color = Color.yellow;
					break;
				case < 1f:
					_musicProgressbar.color = Color.green;
					break;
			}
			OnMusicChanged?.Invoke(_musicProgressbar.fillAmount);
		}

		public void SetEffectsProgress(float value = 1f)
		{
			_effectsProgressbar.fillAmount = value;
			switch (_effectsProgressbar.fillAmount)
			{
				case < .2f:
					_effectsIcon.sprite = _effectsOff;
					break;
				case < .4f:
					_effectsIcon.sprite = _effectsOn;
					_effectsProgressbar.color = Color.red;
					break;
				case < .8f:
					_effectsProgressbar.color = Color.yellow;
					break;
				case < 1f:
					_effectsProgressbar.color = Color.green;
					break;
			}
			OnEffectsChanged?.Invoke(_effectsProgressbar.fillAmount);
		}
	}
}
