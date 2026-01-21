using MatrixUtils.DependencyInjection;
using UnityEngine;

public interface IHighScoreWriter
{
    void UpdateDistance(float distance);
    void UpdateExtraPoints(float extraPoints);
    void CommitScore();
    void ResetScore();
}