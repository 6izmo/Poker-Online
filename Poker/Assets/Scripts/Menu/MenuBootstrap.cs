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

		private void Awake()
        {
            new MenuPresenter(_menuView, _photonConnecter);

			SettingsModel settings = SettingsSaveLoadUtils.LoadSettingsData();
			print(settings);
			_settingsView.Init(settings);
			_settingsPresenter.Init(_settingsView, settings);
		}
	}
}
