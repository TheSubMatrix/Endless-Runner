using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[Serializable]
public class TapResponse : IInputResponse
{
    [SerializeField] InputActionReference m_tapAction;
    [field: SerializeField] public UnityEvent Response { get; private set; }
    
    public void Initialize()
    {
        m_tapAction.action.Enable();
        m_tapAction.action.performed += Respond;
    }
    public void Cleanup()
    {
        m_tapAction.action.performed -= Respond;
        m_tapAction.action.Disable();
    }

    public void Respond(InputAction.CallbackContext _)
    {
        if(!Enabled) return;
        Response.Invoke();
    }
    public bool Enabled { get; set; } = true;
    public UnityEvent OnResponse => Response;
}
