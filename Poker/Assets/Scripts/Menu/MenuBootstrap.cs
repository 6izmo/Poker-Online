using Settings;
using Utilities;
using UnityEngine;
using UnityEngine.LowLevel;
using Cysharp.Threading.Tasks;

namespace Menu
{
    public class MenuBootstrap : MonoBehaviour
    {
		[SerializeField] private MenuView _menuView;
        [Space]
		[SerializeField] private SettingsPresenter _settingsPresenter;
		[SerializeField] private SettingsView _settingsView;

        private void Start()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref loop);

            SettingsModel settings = SettingsSaveLoadUtils.LoadSettingsData();
			_settingsPresenter.Init(_settingsView, settings);
			_settingsView.Init(settings); 

			new MenuPresenter(_menuView);
		}
    }
}
