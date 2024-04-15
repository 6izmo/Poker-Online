using System.Threading.Tasks;

namespace Menu
{
    public class MenuPresenter
    {
        private MenuView _menuView;

		public MenuPresenter(MenuView menuView)
        {
            _menuView = menuView;

			_menuView.OnPlayButtonClicked += LoadRoomsScene;
			_menuView.OnInputedName += TryConnect;
        }

        private async void TryConnect(string playerName)
        {
            bool response = PhotonConnecter.Instance.TryPlayerConnect(playerName);
            _menuView.SwitchToConnectionPanel(true);
            await Task.Delay(2000);
            if (response)
                _menuView.ActivateMainMenu();
            else
                _menuView.ActivateRepeatButton();
		}

		private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
