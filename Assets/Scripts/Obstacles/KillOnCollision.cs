using UnityEngine;

public class KillOnCollision : MonoBehaviour
{
    [SerializeField] uint m_damageToDeal = 1;
    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(m_damageToDeal);
            }
        }
    }
}
