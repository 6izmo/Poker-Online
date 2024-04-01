using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DissolveEffect : MonoBehaviour
{
    [Header("Dissolve")]
    [SerializeField] private float _dissolveTime = 1f;
    [SerializeField] private Material _dissolveMaterial;

    private Image _image;

    private static readonly int Fade = Shader.PropertyToID("_Fade");

    private bool _dissolved = false;
    private float _elapsedTime = 0f;

    public bool Dissolved => _dissolved;
    public Material DissolveMaterial => _dissolveMaterial;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.material = _dissolveMaterial;
    }

    public async Task Dissolve(bool isAppearance)
    {
        _dissolved = false;
        float fadeMaterial = isAppearance ? 0 : 1f;
        float fadeAbs = isAppearance ? 1f : 0f;
        float percent = isAppearance ? (1 / _dissolveTime) : -(1 / _dissolveTime);

        while (_elapsedTime < _dissolveTime)
        {
            fadeMaterial += percent * Time.deltaTime;
            _elapsedTime += Time.deltaTime;
            _image.material.SetFloat(Fade, fadeMaterial);
            await Task.Yield();
        }
        _elapsedTime = 0f;
        _image.material.SetFloat(Fade, fadeAbs);
        _dissolved = true;
        await Task.CompletedTask;
    }
}
