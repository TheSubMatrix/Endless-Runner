using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CoinPickup : MonoBehaviour
{
    public UnityEvent OnPickup = new();
    [SerializeField] uint m_pointsToGive = 20;
    [Inject]IScoreReaderWriter m_scoreManager;
    public void OnTriggerEnter2D(Collider2D other)
    {
        OnPickup.Invoke();
        m_scoreManager.UpdateExtraPoints(m_scoreManager.GetCurrentScore().ExtraPoints + m_pointsToGive);
        gameObject.SetActive(false);
    }
}
