using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class CoinPickup : MonoBehaviour
{
    public UnityEvent OnPickup = new();
    [Inject]IScoreReaderWriter m_scoreManager;
    public void OnTriggerEnter2D(Collider2D other)
    {
        OnPickup.Invoke();
        m_scoreManager.UpdateExtraPoints(m_scoreManager.GetCurrentScore().ExtraPoints + 20);
        gameObject.SetActive(false);
    }
}
