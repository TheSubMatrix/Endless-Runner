using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class HighScoreManager : MonoBehaviour, IHighScoreWriter, IHighScoreReader, IDependencyProvider
{
    [Provide, UsedImplicitly]
    IHighScoreWriter GetScoreWriter() => this;
    
    [Provide, UsedImplicitly]
    IHighScoreReader GetHighScoreReader() => this;
    
    [SerializeReference, ClassSelector] IScoreRepository m_repository;
    
    ScoreData m_currentHighScore = new();
    ScoreData m_currentScore = new();
    void Awake()
    {
        m_currentHighScore = m_repository.Load();
    }

    public void UpdateDistance(float distance)
    {
        m_currentScore.Distance = distance;
    }

    public void UpdateExtraPoints(float extraPoints)
    {
        m_currentScore.ExtraPoints = extraPoints;
    }

    public void CommitScore()
    {
        if (m_currentScore.Total > m_currentHighScore.Total)
        {
            m_currentHighScore = m_currentScore;
            m_repository.Save(m_currentHighScore);
        }
        
        ResetScore();
    }

    public void ResetScore()
    {
        m_currentScore = new();
    }
    
    public ScoreData GetHighScore() => m_currentHighScore;
}