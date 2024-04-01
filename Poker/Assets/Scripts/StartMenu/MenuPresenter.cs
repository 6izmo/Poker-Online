namespace Menu
{
    public class MenuPresenter
    {
        public MenuPresenter(MenuView menuView, PhotonConnecter photonConnecter)
        {
            menuView.Init();

            menuView.OnPlayButtonClicked += LoadRoomsScene;
            menuView.OnInputedName += photonConnecter.PlayerConect;
        }

        private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
