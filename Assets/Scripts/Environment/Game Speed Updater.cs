using System.Collections;
using MatrixUtils.EventBus;
using UnityEngine;
using UnityEngine.Events;

public class GameSpeedUpdater : MonoBehaviour
{
    public UnityEvent<float> OnSpeedOverrideStarted = new();
    public UnityEvent<float> OnSpeedOverrideEnded = new();
    
    Coroutine m_overrideRoutine;
    float m_multiplier = 1f;
    float m_currentOverridePercentage = 1f;
    
    bool GameSpeedOverridden => m_overrideRoutine != null;
    IEventBinding<OverrideGameSpeed> m_overrideGameSpeed;
    
    void OnEnable()
    {
        m_overrideGameSpeed = new EventBinding<OverrideGameSpeed>(OverrideGameSpeed);
        EventBus<OverrideGameSpeed>.Register(m_overrideGameSpeed);
    }

    void OnDisable()
    {
        EventBus<OverrideGameSpeed>.Deregister(m_overrideGameSpeed);
    }
    
    public void UpdateNormalGameSpeed(float distanceTraveled)
    {
        m_multiplier = Mathf.Max(1, 1 + (Mathf.Abs(distanceTraveled/500)));
        if (!GameSpeedOverridden)
        {
            Time.timeScale = m_multiplier;
        }
        else
        {
            Time.timeScale = m_multiplier * m_currentOverridePercentage;
        }
    }

    void OverrideGameSpeed(OverrideGameSpeed overrideGameSpeed)
    {
        CancelOverrideGameSpeed();
        m_overrideRoutine = StartCoroutine(OverrideGameSpeedAsync(overrideGameSpeed.Duration, overrideGameSpeed.SpeedPercentage));
    }

    IEnumerator OverrideGameSpeedAsync(float duration, float speedPercentage)
    {
        float transitionDuration = duration / 10f;
        float startPercentage = m_currentOverridePercentage;
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            m_currentOverridePercentage = Mathf.Lerp(startPercentage, speedPercentage, t);
            Time.timeScale = m_multiplier * m_currentOverridePercentage;
            OnSpeedOverrideStarted.Invoke(m_currentOverridePercentage);
        }
        
        m_currentOverridePercentage = speedPercentage;
        Time.timeScale = m_multiplier * speedPercentage;
        OnSpeedOverrideStarted.Invoke(speedPercentage);
        float scaledDuration = (duration - 2 * transitionDuration) * speedPercentage;
        yield return new WaitForSeconds(scaledDuration);
        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            m_currentOverridePercentage = Mathf.Lerp(speedPercentage, 1f, t);
            Time.timeScale = m_multiplier * m_currentOverridePercentage;
            OnSpeedOverrideEnded.Invoke(m_currentOverridePercentage);
        }
        
        m_currentOverridePercentage = 1f;
        m_overrideRoutine = null;
        Time.timeScale = m_multiplier;
        OnSpeedOverrideEnded.Invoke(1f);
    }

    void CancelOverrideGameSpeed()
    {
        if (!GameSpeedOverridden) return;
        StopCoroutine(m_overrideRoutine);
        m_currentOverridePercentage = 1f;
        m_overrideRoutine = null;
        Time.timeScale = m_multiplier;
        OnSpeedOverrideEnded.Invoke(1f);
    }
}