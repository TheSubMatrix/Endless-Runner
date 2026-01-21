public interface IScoreRepository
{
    ScoreData Load();
    void Save(ScoreData score);
}