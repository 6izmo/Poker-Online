using UnityEngine;

namespace Menu
{
    public class MenuBootstrap : MonoBehaviour
    {
        [SerializeField] private MenuView _menuView;
        [Space]
        [SerializeField] private PhotonConnecter _photonConnecter;

        private void Awake()
        {
            MenuPresenter menuPresenter = new(_menuView, _photonConnecter);
        }
    }
}
