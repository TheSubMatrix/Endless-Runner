using MatrixUtils.DependencyInjection;
using UnityEngine;

public class DistanceScoreProxy : MonoBehaviour
{
    [Inject] IHighScoreWriter m_highScoreWriter;
    float m_distance;
    public void UpdateDistance(float distance)
    {
        m_distance = distance;
    }
    public void GameEnded()
    {
        m_highScoreWriter.UpdateDistance(m_distance);
        m_distance = 0f;
    }
}
