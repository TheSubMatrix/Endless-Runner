public interface IScoreWriter
{
    void UpdateDistance(float distance);
    void UpdateExtraPoints(float extraPoints);
    void CommitScore();
    void ResetCurrentScore();
}