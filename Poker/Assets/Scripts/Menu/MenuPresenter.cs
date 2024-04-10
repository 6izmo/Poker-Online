using Settings;

namespace Menu
{
    public class MenuPresenter
    {
        private PhotonConnecter _photonConnecter;
        private SettingsPresenter _settingsPresenter;

		public MenuPresenter(MenuView menuView, PhotonConnecter photonConnecter, SettingsPresenter settingsPresenter, bool isConnected)
        {
            _photonConnecter = photonConnecter;
			_settingsPresenter = settingsPresenter;
			menuView.Init(isConnected);

			menuView.OnPlayButtonClicked += LoadRoomsScene;
            menuView.OnInputedName += ConnectPlayer;
        }

        private void ConnectPlayer(string nickname)
        {
            _photonConnecter.PlayerConnect(nickname);
            _settingsPresenter.OnConnectedPlayer();
		}

		private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
