using MatrixUtils.EventBus;
using UnityEngine;

public readonly struct OverrideGameSpeed : IEvent
{
    public readonly float SpeedPercentage;
    public readonly float Duration;
    public OverrideGameSpeed(float speedPercentage, float duration)
    {
        SpeedPercentage = speedPercentage;
        Duration = duration;
    }
}