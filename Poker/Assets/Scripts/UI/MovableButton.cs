using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace UIVisual
{
    public class MovableButton : AppearSmoothly, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Vector3 _endScaleValue;

        public void OnPointerEnter(PointerEventData eventData) => transform.DOScale(_endScaleValue, Duration);

        public void OnPointerExit(PointerEventData eventData) => transform.DOScale(DefaultScale, Duration);
    }
}
