using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class RunnerPlayerController : MonoBehaviour, IInputHandler, IDependencyProvider
{
    [Provide, UsedImplicitly] IInputHandler GetInputHandler() => this;
    [SerializeField] SwipeResponse Crouch;
    [SerializeField] SwipeResponse Jump;
    [SerializeField] TapResponse Swing;

    UnityEvent IInputHandler.Jump => Jump.Response;
    UnityEvent IInputHandler.Crouch => Crouch.Response;
    UnityEvent IInputHandler.Swing => Swing.Response;

    void Awake()
    {
        Crouch.Initialize();
        Jump.Initialize();
        Swing.Initialize();
    }
}