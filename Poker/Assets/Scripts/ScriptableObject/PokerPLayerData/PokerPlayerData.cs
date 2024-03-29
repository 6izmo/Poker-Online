using System;
using Players;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPlayerData")]
public class PokerPlayerData : ScriptableObject
{
    [SerializeField] private PlayerInfoView _playerItemPrefab;
    [SerializeField] private PlayerView _playerViewPrefab;
    [SerializeField] private PlayerItemPosition _localPlayerPosition;
    [SerializeField] private List<PlayerItemPosition> _positions = new();

    private LinkedList<PlayerItemPosition> _linkedItemPos;

    public PlayerItemPosition LocalPosition => _localPlayerPosition;
    public PlayerItemPosition GetPosition(int index) => _linkedItemPos.ElementAtOrDefault(index);

    public void Init(int actorNumber)
    {
        _linkedItemPos = new(_positions);
        SortPositionList(actorNumber);
    }

    private void SortPositionList(int value)
    {
        if(value == 2)
            _linkedItemPos.AddFirst(_linkedItemPos.RemoveAndGetLast());
        else if (value == 3)
            _linkedItemPos.AddLast(_linkedItemPos.RemoveAndGetFirst());
    }

    public PlayerInfoView PlayerItemPrefab => _playerItemPrefab;

    public PlayerView PlayerViewPrefab => _playerViewPrefab;
}

[Serializable]
public class PlayerItemPosition 
{
    public Vector2 Positions;
    public float Rotation;
    public float Radius = 360;
}

