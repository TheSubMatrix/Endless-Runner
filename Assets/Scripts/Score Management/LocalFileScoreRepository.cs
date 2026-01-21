using UnityEngine;
using System;
using System.IO;
using JetBrains.Annotations;

[Serializable]
public class LocalFileScoreRepository : IScoreRepository
{
    static string FilePath => Path.Combine(Application.persistentDataPath, "HighScore.json");
    
    [NotNull]
    public ScoreData Load()
    {
        if (!File.Exists(FilePath))
        {
            return new();
        }

        string json = File.ReadAllText(FilePath);
    
        if (string.IsNullOrWhiteSpace(json))
        {
            return new();
        }
    
        ScoreData score = JsonUtility.FromJson<ScoreData>(json);
        return score ?? new ScoreData();
    }

    public void Save(ScoreData score)
    {
        Debug.Log(score.Total);
        string json = JsonUtility.ToJson(score, prettyPrint: true);
        string directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(FilePath, json);
    }
}