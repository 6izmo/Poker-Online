using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace UIVisual
{
    public class MovableButton : AppearSmoothly, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Vector3 _endScaleValue;

        private void ToDefault () => Tween = transform.DOScale(DefaultScale, Duration);

		public void OnPointerEnter(PointerEventData eventData) => Tween = transform.DOScale(_endScaleValue, Duration);

		public void OnPointerDown(PointerEventData eventData) => ToDefault();

		public void OnPointerExit(PointerEventData eventData) => ToDefault();
    }
}
