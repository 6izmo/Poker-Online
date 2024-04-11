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
		private SettingsModel _model;

		public void Init(SettingsView view, SettingsModel model)
		{
			_view = view;
			_model = model;

			_view.OnClosed += SaveAudio;
			_view.OnEffectsChanged += ChangeEffectsMixer;
			_view.OnMusicChanged += ChangeMusicMixer;
		}

		private void ChangeMusicMixer(float value) => _musicMixer.SetFloat("MusicVolume", Mathf.Lerp(-64, 0, value));

		private void ChangeEffectsMixer(float value) => _effectsMixer.SetFloat("EffectsVolume", Mathf.Lerp(-64, 0, value));

		private void SaveAudio(float music, float audio)
		{
			_model.Music = music;
			_model.Effects = audio;
			SettingsSaveLoadUtils.SaveSettingsData(_model);
		}

		public void OnConnectedPlayer()
		{
			_model.IsConnected = true;
			SettingsSaveLoadUtils.SaveSettingsData(_model);
		}

		private void OnDestroy()
		{
			_view.OnClosed -= SaveAudio;
			_view.OnEffectsChanged -= ChangeEffectsMixer;
			_view.OnMusicChanged -= ChangeMusicMixer;
		}
	}
}
