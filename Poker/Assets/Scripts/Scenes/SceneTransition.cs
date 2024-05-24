using Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : Singleton<SceneTransition>
{
    [SerializeField] private DissolveEffect _dissolveEffect;

    private AsyncOperation _asyncOperation;
    private static bool _isOpeningAnimation;

    private async void Start()
    {
        if (_isOpeningAnimation)
			await Instance._dissolveEffect.Dissolve(false);
    }

    public static async void SwitchToScene(string sceneName)
    {
        if (Instance == null) 
            return;
        Instance._asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        Instance._asyncOperation.allowSceneActivation = false;
        await Instance._dissolveEffect.Dissolve(true);
        _isOpeningAnimation = true;
        Instance._asyncOperation.allowSceneActivation = true;
    }

    public static async void SwitchToScene(int sceneId) 
    {
		if (Instance == null)
			return;
		Instance._asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        Instance._asyncOperation.allowSceneActivation = false;
        await Instance._dissolveEffect.Dissolve(true);
        _isOpeningAnimation = true;
        Instance._asyncOperation.allowSceneActivation = true;
    }
}
