using UnityEngine;
using UnityEngine.Events;

public interface IInputHandler
{
    public UnityEvent Jump { get; }
    public UnityEvent Roll { get; }
    public UnityEvent Swing { get; }
}
