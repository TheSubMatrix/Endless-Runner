using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    [SerializeField, RequiredField] TMP_Text m_scoreText;
    [SerializeField, RequiredField] TMP_Text m_highScoreText;
    [Inject, UsedImplicitly] IScoreReader m_scoreReader;

    void Start()
    {
        m_scoreText.text = "Score: " + m_scoreReader.GetLatestScore().Total;
        m_highScoreText.text = "High Score: " + m_scoreReader.GetHighScore().Total;
    }
}
