using UnityEngine;

namespace RoomList
{
    public class RoomsBootstrap : MonoBehaviour
    {
        [SerializeField] private RoomListPresenter _listPresenter;
        [SerializeField] private RoomListView _listView;
        [Space]
        [SerializeField] private RoomItem _roomItem;

        private void Awake()
        {
            RoomListModel listModel = new(_roomItem);
            _listView.Init();
            _listPresenter.Init(listModel, _listView);
        }
    }
}
