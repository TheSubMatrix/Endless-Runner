using System;
using MatrixUtils.EventBus;
using UnityEngine;

public class SlowMotionPowerUp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        EventBus<OverrideGameSpeed>.Raise(new(0.5f, 10));
        gameObject.SetActive(false);
    }
}
