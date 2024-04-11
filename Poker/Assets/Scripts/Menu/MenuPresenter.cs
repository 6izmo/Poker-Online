using Settings;

namespace Menu
{
    public class MenuPresenter
    {
        private PhotonConnecter _photonConnecter;
        private SettingsPresenter _settingsPresenter;
        private MenuView _menuView;

		public MenuPresenter(MenuView menuView, PhotonConnecter photonConnecter, SettingsPresenter settingsPresenter)
        {
            _photonConnecter = photonConnecter;
			_settingsPresenter = settingsPresenter;
            _menuView = menuView;

			_menuView.OnPlayButtonClicked += LoadRoomsScene;
			_menuView.OnInputedName += ConnectPlayer;
        }

        private void ConnectPlayer(string nickname)
        {
            _photonConnecter.PlayerConnect(nickname);
            _settingsPresenter.OnConnectedPlayer();
		}

		private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
