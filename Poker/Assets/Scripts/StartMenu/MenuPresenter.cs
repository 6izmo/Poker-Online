namespace Menu
{
    public class MenuPresenter
    {
        public MenuPresenter(MenuView menuView, MusicPlayer music, PhotonConnecter photonConnecter)
        {
            menuView.Init();

            menuView.OnPlayButtonClicked += LoadRoomsScene;
            menuView.OnInputedName += photonConnecter.PlayerConect;
            menuView.OnMutedMusic += music.Mute;
        }

        private void LoadRoomsScene() => SceneTransition.SwitchToScene("Rooms");
    }
}
