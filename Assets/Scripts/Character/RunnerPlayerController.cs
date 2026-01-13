using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RunnerPlayerController : MonoBehaviour, IInputHandler, IDependencyProvider
{
    [Provide, UsedImplicitly] IInputHandler GetInputHandler() => this;
    [SerializeField] SwipeResponse Crouch;
    [SerializeField] SwipeResponse Jump;
    [SerializeField] InputActionReference Swing;

    UnityEvent IInputHandler.Jump => Jump.Response;
    UnityEvent IInputHandler.Crouch => Crouch.Response;
    UnityEvent IInputHandler.Swing => throw new NotImplementedException();

    private void Awake()
    {
        Crouch.Initialize(value => value.magnitude > 5 && Vector2.Dot(value.normalized, Vector2.down) > 0.5);
        Jump.Initialize(value => value.magnitude > 5 && Vector2.Dot(value.normalized, Vector2.up) > 0.5);
    }
}
[Serializable]
public class SwipeResponse
{
    Vector2 InputValue;
    [SerializeField] InputActionReference SwipeAction;
    [SerializeField] InputActionReference TapAction;
    public Predicate<Vector2> SwipeFilter;
    [field: SerializeField] public UnityEvent Response { get; private set; } = new();
    public void Initialize(Predicate<Vector2> swipeFilter = null)
    {
        SwipeAction.action.Enable();
        TapAction.action.Enable();
        SwipeAction.action.performed += ProcessAction;
        TapAction.action.canceled += ConfirmAction;
        SwipeFilter = swipeFilter;
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
        InputValue = SwipeAction.action.ReadValue<Vector2>();
    }
    void ConfirmAction(InputAction.CallbackContext context)
    {
        if (SwipeFilter.Invoke(InputValue))
        {
            Response.Invoke();
        }
    }
}