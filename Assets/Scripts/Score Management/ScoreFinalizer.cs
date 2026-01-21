using MatrixUtils.DependencyInjection;
using UnityEngine;

public class ScoreFinalizer : MonoBehaviour
{
    [Inject] IHighScoreWriter m_highScoreWriter;
    public void FinalizeScore()
    {
        m_highScoreWriter.CommitScore();
    }
}
