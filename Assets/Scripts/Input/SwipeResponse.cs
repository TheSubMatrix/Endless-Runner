using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class SwipeResponse: IInputResponse
{
    [SerializeField] float m_swipeAngle;
    [SerializeField] float m_swipeStrengthThreshold = 5f;
    [SerializeField] float m_swipeAngleThreshold = 45f;
    Vector2 m_validSwipeDirection = Vector2.zero;
    float m_swipeValidDot;
    Vector2 m_inputValue;
    bool m_registeredTap;
    [SerializeField] InputActionReference DeltaAction;
    [SerializeField] InputActionReference HoldAction;
    [SerializeField] InputActionReference TapAction;
    [field: SerializeField] public UnityEvent Response { get; private set; } = new();
    public void Initialize()
    {
        DeltaAction.action.Enable();
        HoldAction.action.Enable();
        TapAction.action.Enable();
        HoldAction.action.started += ClearTap;
        DeltaAction.action.performed += ProcessAction;
        TapAction.action.performed += RegisterTap;
        HoldAction.action.canceled += ConfirmAction;
        m_validSwipeDirection = new(Mathf.Cos(m_swipeAngle * Mathf.Deg2Rad), Mathf.Sin(m_swipeAngle * Mathf.Deg2Rad));
        m_swipeValidDot = Mathf.Cos(m_swipeAngleThreshold * Mathf.Deg2Rad);
    }
    public void Cleanup()
    {
        DeltaAction.action.performed -= ProcessAction;
        HoldAction.action.canceled -= ConfirmAction;
        TapAction.action.performed -= RegisterTap;
        DeltaAction.action.Disable();
        HoldAction.action.Disable();
        TapAction.action.Disable();
    }
    void ProcessAction(InputAction.CallbackContext context)
    {
        m_inputValue += DeltaAction.action.ReadValue<Vector2>();
    }
    void ConfirmAction(InputAction.CallbackContext context)
    {
        if (m_inputValue.magnitude > m_swipeStrengthThreshold && Vector2.Dot(m_inputValue.normalized, m_validSwipeDirection.normalized) > m_swipeValidDot && !m_registeredTap && Enabled)
        {
            Response.Invoke();
        }
        m_inputValue = Vector2.zero;
        m_registeredTap = false;
    }

    void RegisterTap(InputAction.CallbackContext context)
    {
        m_registeredTap = true;
    }
    void ClearTap(InputAction.CallbackContext context)
    {
        m_registeredTap = false;
    }

    public bool Enabled { get; set; } = true;
    public UnityEvent OnResponse => Response;
}
