using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private CurvedText _nickName;
    private RectTransform _rectTransform;

    [Header("Icon")]
    [SerializeField] private Image _readyConditionIcon;
    [SerializeField] private Sprite _readySprite;   
    [SerializeField] private Sprite _unreadySprite;

    public bool Ready { get; private set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        SetReadyCondition(false);
    }

    public void SetPLayerInfo(Player player)
    {
        _nickName.text = player.NickName;
        gameObject.name = player.NickName;
	}

    public void ChangeColorText(Color color)
    {
        Color newColor = _nickName.color == Color.white ? color : Color.white;
        _nickName.color = newColor;
    }

	public void SetPosition(PlayerItemPosition position, RectTransform parent)
    {
        _rectTransform.SetParent(parent);
        _rectTransform.anchoredPosition = position.Positions;
        _nickName.Radius = position.Radius;
        _rectTransform.localRotation = Quaternion.Euler(0, 0, position.Rotation);       
    }

    public void SetReadyCondition(bool condition)
    {
        Ready = condition;

        _readyConditionIcon.sprite = condition ? _readySprite : _unreadySprite;
        _readyConditionIcon.color = condition ? Color.green : Color.red;
    }

    public void HideReadyIcon() => _readyConditionIcon.Deactivate();
}
