using UnityEngine;
using UnityEngine.Events;

public interface IInputResponse
{
    public bool Enabled { get; set; }
    public UnityEvent OnResponse{get;}
}
