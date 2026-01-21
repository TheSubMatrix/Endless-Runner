using MatrixUtils.DependencyInjection;
using UnityEngine;

public class SceneTransitionProxy : MonoBehaviour, ISceneTransitioner
{
    [Inject]
    ISceneTransitioner m_sceneTransitionerInstance;
    
    public void RequestTransitionTo(string sceneName, float transitionTime)
    {
        m_sceneTransitionerInstance.RequestTransitionTo(sceneName);
    }

    public void RequestTransitionTo(string sceneName)
    {
        m_sceneTransitionerInstance.RequestTransitionTo(sceneName);
    }
    
    public void ReloadScene(float transitionTime)
    {
        m_sceneTransitionerInstance.ReloadScene(transitionTime);
    }

    public void ReloadScene()
    {
        m_sceneTransitionerInstance.ReloadScene();
    }
}