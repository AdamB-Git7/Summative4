using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ScoreUtility
{
    // Store the score file name in one place.
    private const string ScoreFileName = "scores.txt";

    // Store how many scores the UI should keep.
    private const int MaxScoresToKeep = 5;

    public static string GetScoreFilePath()
    {
        // Build the full persistent-data path used for score storage.
        return Path.Combine(Application.persistentDataPath, ScoreFileName);
    }

    public static void SaveScore(string playerName, int score)
    {
        // Build one CSV line containing the player name, score, and timestamp.
        string entry = $"{playerName},{score},{DateTime.Now:yyyy-MM-dd HH:mm}";

        // Append the new score entry to the score file.
        File.AppendAllText(GetScoreFilePath(), entry + Environment.NewLine);

        // Log the saved score path for debugging.
        Debug.Log($"Score saved: {entry} -> {GetScoreFilePath()}");
    }

    public static List<ScoreEntry> LoadTopScores()
    {
        // Store the final score list that will be returned.
        List<ScoreEntry> entries = new();

        // Store the current score file path.
        string filePath = GetScoreFilePath();

        // Return an empty list if no score file exists yet.
        if (!File.Exists(filePath))
        {
            return entries;
        }

        // Visit each saved line in the score file.
        foreach (string line in File.ReadAllLines(filePath))
        {
            // Skip blank or whitespace-only lines.
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Split the CSV line into pieces.
            string[] parts = line.Split(',');

            // Skip malformed lines or lines with invalid scores.
            if (parts.Length < 2 || !int.TryParse(parts[1], out int score))
            {
                continue;
            }

            // Add the parsed score entry to the list.
            entries.Add(new ScoreEntry { playerName = parts[0], score = score });
        }

        // Sort the list from highest score to lowest score.
        entries.Sort((left, right) => right.score.CompareTo(left.score));

        // Remove extra rows if there are more than the UI wants to show.
        if (entries.Count > MaxScoresToKeep)
        {
            entries.RemoveRange(MaxScoresToKeep, entries.Count - MaxScoresToKeep);
        }

        // Return the sorted and trimmed score list.
        return entries;
    }

    public static string BuildHighScoreDisplayText(List<ScoreEntry> entries)
    {
        // Return a friendly empty-state message when no scores exist yet.
        if (entries == null || entries.Count == 0)
        {
            return "No scores yet.\nPlay a game to set your first record!";
        }

        // Build the multi-line score text efficiently.
        StringBuilder builder = new();

        // Add the score list heading.
        builder.AppendLine("=== High Scores ===");
        builder.AppendLine();

        // Add each ranked score line.
        for (int index = 0; index < entries.Count; index++)
        {
            builder.AppendLine($"{index + 1}.  {entries[index].playerName}  -  {entries[index].score} pts");
        }

        // Return the final display text.
        return builder.ToString();
    }
}
