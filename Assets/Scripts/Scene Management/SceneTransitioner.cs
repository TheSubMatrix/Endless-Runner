using System.Collections;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class SceneTransitioner : PersistentService<ISceneTransitioner>, ISceneTransitioner
{
    
    public bool IsTransitioning { get; private set; }
    CanvasGroup m_screenFadeGroup;
    [Provide, UsedImplicitly]
    ISceneTransitioner ProvideSceneTransitionManager() => this;

    void Awake()
    {
        m_screenFadeGroup = GetComponent<CanvasGroup>();
        m_screenFadeGroup.alpha = 0;
        m_screenFadeGroup.interactable = false;
        m_screenFadeGroup.blocksRaycasts = false;
    }
    public void RequestTransitionTo(string sceneName, float transitionTime = 1f)
    {
        if(IsTransitioning) return;
        IsTransitioning = true;
        StartCoroutine(TransitionSceneAsync(sceneName, transitionTime));
    }

    IEnumerator TransitionSceneAsync(string sceneName, float transitionTime = 1f)
    {
        yield return FadeCanvasGroupAsync(m_screenFadeGroup, 1, transitionTime / 2);
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return FadeCanvasGroupAsync(m_screenFadeGroup, 0, transitionTime / 2);
        IsTransitioning = false;
    }

    static IEnumerator FadeCanvasGroupAsync(CanvasGroup canvasGroup, float desiredAlpha, float duration = 0.5f)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0;
        while (elapsedTime <= duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, desiredAlpha, elapsedTime / duration);
            yield return null;
            elapsedTime +=  Time.deltaTime;
        }
        canvasGroup.alpha = desiredAlpha;
        canvasGroup.interactable = canvasGroup.alpha > 0;
        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
    }
}
