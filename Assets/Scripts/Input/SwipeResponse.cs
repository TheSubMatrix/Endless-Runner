using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[Serializable]
public class SwipeResponse
{
    [SerializeField] float m_swipeAngle;
    [SerializeField] float m_swipeStrengthThreshold = 5f;
    [SerializeField] float m_swipeAngleThreshold = 45f;
    Vector2 m_validSwipeDirection = Vector2.zero;
    float m_swipeValidDot;
    Vector2 m_inputValue;
    [SerializeField] InputActionReference SwipeAction;
    [SerializeField] InputActionReference TapAction;
    [field: SerializeField] public UnityEvent Response { get; private set; } = new();
    public void Initialize()
    {
        SwipeAction.action.Enable();
        TapAction.action.Enable();
        SwipeAction.action.performed += ProcessAction;
        TapAction.action.canceled += ConfirmAction;
        m_validSwipeDirection = new(Mathf.Cos(m_swipeAngle * Mathf.Deg2Rad), Mathf.Sin(m_swipeAngle * Mathf.Deg2Rad));
        m_swipeValidDot = Mathf.Cos(m_swipeAngleThreshold * Mathf.Deg2Rad);
    }
    public void Cleanup()
    {
        SwipeAction.action.performed -= ProcessAction;
        TapAction.action.canceled -= ConfirmAction;
        SwipeAction.action.Disable();
        TapAction.action.Disable();
    }
    void ProcessAction(InputAction.CallbackContext context)
    {
        m_inputValue += SwipeAction.action.ReadValue<Vector2>();
    }
    void ConfirmAction(InputAction.CallbackContext context)
    {
        if (m_inputValue.magnitude > m_swipeStrengthThreshold && Vector2.Dot(m_inputValue.normalized, m_validSwipeDirection.normalized) > m_swipeValidDot)
        {
            Response.Invoke();
        }
        m_inputValue = Vector2.zero;
    }
}
