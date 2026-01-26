using System;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] Observer<uint> m_health = new(1);
    [SerializeField] UnityEvent m_onDeath = new();
    bool m_isDead;
    public void Damage(uint damage)
    {
        if ((m_health.Value = damage >= m_health.Value ? 0 : m_health.Value - damage) > 0 || m_isDead) return;
        m_onDeath.Invoke();
        m_isDead = true;
    }
}
