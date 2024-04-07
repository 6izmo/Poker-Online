using DG.Tweening;
using UnityEngine;

namespace UIVisual
{
    public class AppearSmoothly : MonoBehaviour
    {
        [SerializeField] protected float Duration = 0.5f;
        protected Vector3 DefaultScale;
        protected Tween Tween;

        private void Awake() => DefaultScale = transform.localScale;

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;  
            Tween = transform.DOScale(DefaultScale, Duration);
        }

        private void OnDisable() => Tween?.Kill();
    }
}
