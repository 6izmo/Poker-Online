using Utilities;
using UnityEngine; 
using UnityEngine.SceneManagement;

public class SceneTransition : Singleton<SceneTransition>
{
    [SerializeField] private DissolveEffect _dissolveEffect;
    [SerializeField] private GameObject _contentPanel;

    private AsyncOperation _asyncOperation;
    private static bool _isOpeningAnimation;

    private async void Start()
    {
        if (_isOpeningAnimation)
        {
            await Instance._dissolveEffect?.Dissolve(false);
            Instance._contentPanel.Deactivate();
            Instance._dissolveEffect?.Deactivate();
        }
    }

    public static async void SwitchToScene(string sceneName)
    {
        Instance._dissolveEffect?.Activate();
        Instance._asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        Instance._asyncOperation.allowSceneActivation = false;
        Instance._contentPanel.Activate();
        await Instance._dissolveEffect?.Dissolve(true);
        _isOpeningAnimation = true;
        Instance._asyncOperation.allowSceneActivation = true;
    }

    public static async void SwitchToScene(int sceneId)
    {
        Instance._dissolveEffect.Activate();
        Instance._asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        Instance._asyncOperation.allowSceneActivation = false;
        await Instance._dissolveEffect?.Dissolve(true);
        Instance._contentPanel.Activate();
        _isOpeningAnimation = true;
        Instance._asyncOperation.allowSceneActivation = true;
    }
}
