using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreEntry
{
    public string name;
    public int score;
    public string date; // ISO yyyy-MM-dd
}

[Serializable]
public class Leaderboard
{
    public List<ScoreEntry> entries = new List<ScoreEntry>();
}

public static class LeaderboardManager
{
    const string LeaderboardKey = "leaderboard_v1";
    const string CurrentPlayerKey = "player_name_current";

    // === Nome do jogador atual ===
    public static string CurrentPlayerName
    {
        get => PlayerPrefs.GetString(CurrentPlayerKey, "");
        set { PlayerPrefs.SetString(CurrentPlayerKey, value ?? ""); PlayerPrefs.Save(); }
    }

    // === Load / Save ===
    public static Leaderboard Load()
    {
        var json = PlayerPrefs.GetString(LeaderboardKey, "");
        if (string.IsNullOrEmpty(json)) return new Leaderboard();
        try { return JsonUtility.FromJson<Leaderboard>(json) ?? new Leaderboard(); }
        catch { return new Leaderboard(); }
    }

    public static void Save(Leaderboard lb)
    {
        var json = JsonUtility.ToJson(lb);
        PlayerPrefs.SetString(LeaderboardKey, json);
        PlayerPrefs.Save();
    }

    // === Submeter placar e manter top N ===
    public static void SubmitScore(string name, int score, int maxEntries = 10)
    {
        name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
        var lb = Load();

        lb.entries.Add(new ScoreEntry
        {
            name = name,
            score = score,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });

        // Ordena desc por score, corta no top
        lb.entries.Sort((a, b) => b.score.CompareTo(a.score));
        if (lb.entries.Count > maxEntries) lb.entries.RemoveRange(maxEntries, lb.entries.Count - maxEntries);

        Save(lb);
    }

    public static string FormatLeaderboardText(int maxLines = 10)
    {
        var lb = Load();
        var lines = new List<string>();
        for (int i = 0; i < lb.entries.Count && i < maxLines; i++)
        {
            var e = lb.entries[i];
            lines.Add($"{i + 1,2}. {e.name,-12}  {e.score,6}  ({e.date})");
        }
        if (lines.Count == 0) lines.Add("Sem registros ainda.");
        return string.Join("\n", lines);
    }
}
