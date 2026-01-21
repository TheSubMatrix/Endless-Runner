using System;
using MatrixUtils.EventBus;
using UnityEngine;
using UnityEngine.Events;

public class GameOverNotificationResponder : MonoBehaviour
{
    [SerializeField] UnityEvent m_onGameOver;
    EventBinding<GameOverEvent> m_gameOverEvent;
    void OnEnable()
    {
        m_gameOverEvent = new(CallUnityEvent);
        EventBus<GameOverEvent>.Register(m_gameOverEvent);
    }

    void OnDisable()
    {
        EventBus<GameOverEvent>.Deregister(m_gameOverEvent);
    }

    void CallUnityEvent()
    {
        m_onGameOver.Invoke();
    }
}
