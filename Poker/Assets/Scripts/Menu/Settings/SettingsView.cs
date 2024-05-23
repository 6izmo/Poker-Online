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

		private AudioTrack _music;
		private AudioTrack _effects;

		public void Init(SettingsModel settings)
		{
			_close.Add(() => OnClosed?.Invoke(_musicProgressbar.fillAmount, _effectsProgressbar.fillAmount));
			_plusMusic.Add(() => SetMusicProgress(_musicProgressbar.fillAmount + _progressStep));
			_minusMusic.Add(() => SetMusicProgress(_musicProgressbar.fillAmount - _progressStep));
			_plusEffects.Add(() => SetEffectsProgress(_effectsProgressbar.fillAmount + _progressStep));
			_minusEffects.Add(() => SetEffectsProgress(_effectsProgressbar.fillAmount - _progressStep));

			float musicValue = settings == null ? 1f : settings.Music;
			float effectsValue = settings == null ? 1f : settings.Effects;

			_music = new(_musicProgressbar, _musicIcon, _musicOff, _musicOn);
            _effects = new(_effectsProgressbar, _effectsIcon, _effectsOff, _effectsOn);

            SetMusicProgress(musicValue);
			SetEffectsProgress(effectsValue);
		}

		public void SetMusicProgress(float value)
		{
			_music.SetProgress(value);
			OnMusicChanged?.Invoke(_music.FillAmount);
		}

		public void SetEffectsProgress(float value)
		{
			_effects.SetProgress(value);
			OnEffectsChanged?.Invoke(_effects.FillAmount);
		}  
	}

    public class AudioTrack
    {
		private Image _progressBar;
		private Image _icon;
		private Sprite _on;
		private Sprite _off;

		public float FillAmount => _progressBar.fillAmount;

        public AudioTrack(Image progressBar, Image icon, Sprite off, Sprite on)
        {
			_progressBar = progressBar;
			_icon = icon;
			_on = on;
			_off = off;
        }

		public void SetProgress(float value)
		{
            _progressBar.fillAmount = value;
            switch (_progressBar.fillAmount)
            {
                case < .2f:
                    _icon.sprite = _off;
                    break;
                case < .4f:
                    _icon.sprite = _on;
                    _progressBar.color = Color.red;
                    break;
                case < .8f:
                    _progressBar.color = Color.yellow;
                    break;
                case < 1f:
                    _progressBar.color = Color.green;
                    break;
            }
        }
    }
}
