using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CompletedManager : MonoBehaviour
{
    // This class is responsible for keeping lists of completed puzzles for each difficulty in files and return them when queried. Can store a single puzzle at a time.
    // Won't store a puzzle already stored.
    // File format: this class keeps folder "completed" in Application.persistentDataPath. Inside there's a file for each difficulty. One puzzle per line.
    // Asks DifficultyManager for difficulty every time it operates.

    // PUBLIC
    public static CompletedManager Instance { get; private set; }
    public static void RememberPuzzle(string puzzle)
    {
        if (!GetCompletedPuzzles().Contains(puzzle))
        {
            using StreamWriter writer = new(GetFilePath(), true);
            writer.WriteLine(puzzle);
        }
    }
    public static List<string> GetCompletedPuzzles()
    {
        if (File.Exists(GetFilePath()))
        {
            string[] lines = File.ReadAllLines(GetFilePath());
            return new List<string>(lines);
        }
        return new List<string>();
    }

    // PRIVATE
    private const string COMPLETED_PUZZLES_FOLDER_NAME = "completed";
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, COMPLETED_PUZZLES_FOLDER_NAME));
    }
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, COMPLETED_PUZZLES_FOLDER_NAME, DifficultyManager.Instance.CurrentDifficulty.DifficultyName);
    }
}
