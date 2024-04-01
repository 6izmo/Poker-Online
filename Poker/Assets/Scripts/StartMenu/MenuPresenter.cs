namespace Menu
{
    public class MenuPresenter
    {
        public MenuPresenter(MenuView menuView, PhotonConnecter photonConnecter)
        {
            menuView.Init();

            menuView.OnPlayButtonClicked += LoadRoomsScene;
            menuView.OnInputName += photonConnecter.PlayerConect;
        }

        private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
