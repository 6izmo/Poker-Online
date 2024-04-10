using Settings;
using UnityEngine;
using Utilities;

namespace Menu
{
    public class MenuBootstrap : MonoBehaviour
    {
		[SerializeField] private MenuView _menuView;
        [Space]
        [SerializeField] private PhotonConnecter _photonConnecter;
        [Space]
		[SerializeField] private SettingsPresenter _settingsPresenter;
		[SerializeField] private SettingsView _settingsView;   

		private void Start()
        {
			SettingsModel settings = SettingsSaveLoadUtils.LoadSettingsData();
			_settingsPresenter.Init(_settingsView, settings);
			_settingsView.Init(settings);

			bool isConnected = settings == null ? false : settings.IsConnected;
			new MenuPresenter(_menuView, _photonConnecter, _settingsPresenter, isConnected);
		}
	}
}
