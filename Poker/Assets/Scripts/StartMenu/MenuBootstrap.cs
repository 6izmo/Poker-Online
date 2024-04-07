using UnityEngine;

namespace Menu
{
    public class MenuBootstrap : MonoBehaviour
    {
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private MenuView _menuView;
        [Space]
        [SerializeField] private PhotonConnecter _photonConnecter;

        private void Awake() => new MenuPresenter(_menuView, _musicPlayer, _photonConnecter);
    }
}
