using System;
using UnityEngine;

[Serializable]
public struct ScoreData
{
    public float Distance;
    public float ExtraPoints;

    public ScoreData(float distance = 0, float extraPoints = 0)
    {
        Distance = distance;
        ExtraPoints = extraPoints;
    }
    public int Total => Mathf.RoundToInt(Distance + ExtraPoints);
}
[Serializable]
public class SavedScoreInformation
{
    public ScoreData HighScore;
    public ScoreData LatestScore;

    public SavedScoreInformation(ScoreData highScore, ScoreData latestScore)
    {
        HighScore = highScore;
        LatestScore = latestScore;
    }
    public SavedScoreInformation()
    {
        HighScore = new();
        LatestScore = new();
    }
}