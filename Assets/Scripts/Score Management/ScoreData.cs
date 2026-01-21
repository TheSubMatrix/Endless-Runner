public class ScoreData
{
    public float Distance;
    public float ExtraPoints;

    public ScoreData(float distance = 0, float extraPoints = 0)
    {
        Distance = distance;
        ExtraPoints = extraPoints;
    }
    public float Total => Distance + ExtraPoints;
}
