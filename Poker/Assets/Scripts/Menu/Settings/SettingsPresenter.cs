using Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings
{
	public class SettingsPresenter : MonoBehaviour
	{
		[SerializeField] private AudioMixer _musicMixer;
		[SerializeField] private AudioMixer _effectsMixer;
		private SettingsView _view;

		public void Init(SettingsView view, SettingsModel model)
		{
			_view = view;

			_view.OnClosed += SaveAudio;
			_view.OnEffectsChanged += ChangeEffectsMixer;
			_view.OnMusicChanged += ChangeMusicMixer; 
			
			if(model != null)
			{
				ChangeMusicMixer(model.Music);
				ChangeEffectsMixer(model.Effects);
			}
		}

		private void ChangeMusicMixer(float value) => _musicMixer.SetFloat("MusicVolume", Mathf.Lerp(-64, 0, value));

		private void ChangeEffectsMixer(float value) => _effectsMixer.SetFloat("EffectsVolume", Mathf.Lerp(-64, 0, value));

		private void SaveAudio(float music, float audio)
		{
			SettingsModel settingsModel = new SettingsModel(music, audio);
			SettingsSaveLoadUtils.SaveSettingsData(settingsModel);
		}

		private void OnDisable()
		{
			_view.OnClosed -= SaveAudio;
			_view.OnEffectsChanged -= ChangeEffectsMixer;
			_view.OnMusicChanged -= ChangeMusicMixer;
		}
	}
}
