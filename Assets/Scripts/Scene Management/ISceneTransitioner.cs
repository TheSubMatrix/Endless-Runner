using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneTransitioner
{
    void RequestTransitionTo(string sceneName, float transitionTime = 1f);

    void ReloadScene(float transitionTime = 1f)
    {
        RequestTransitionTo(SceneManager.GetActiveScene().name, transitionTime);
    }
}
