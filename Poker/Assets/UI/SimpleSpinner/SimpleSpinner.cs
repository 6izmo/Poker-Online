using UnityEngine;
using UnityEngine.UI;

namespace UIVisual
{
    [RequireComponent(typeof(Image))]
    public class SimpleSpinner : AppearSmoothly
    {
        [Header("Rotation")]
        [SerializeField, Range(-10, 10)] private float _rotationSpeed = 1;
		[SerializeField] private AnimationCurve _rotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private void Update() => transform.localEulerAngles = new Vector3(0, 0, -360 * _rotationAnimationCurve.Evaluate((_rotationSpeed * Time.time) % 1));
    }
}