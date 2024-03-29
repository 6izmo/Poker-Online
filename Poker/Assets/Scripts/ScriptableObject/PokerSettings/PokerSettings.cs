using UnityEngine;

[CreateAssetMenu(fileName = "PokerSettings")]
public class PokerSettings : ScriptableObject
{
    [SerializeField] private int _smallBlind;
    [SerializeField] private int _bigBlind;
    [Space]
    [SerializeField] private int _startPlayerMoney;

    public int SmallBlind => _smallBlind;
    public int BigBlind => _bigBlind;
    public int StartPlayerMoney => _startPlayerMoney;
}
