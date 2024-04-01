using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MovableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 _endScaleValue;
    [SerializeField] private float _duration;
    private Vector3 _defaultScale;

    private void Awake() => _defaultScale = transform.localScale;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(_defaultScale, _duration);
    }

    public void OnPointerEnter(PointerEventData eventData) => transform.DOScale(_endScaleValue, _duration);

    public void OnPointerExit(PointerEventData eventData) => transform.DOScale(_defaultScale, _duration);
}
