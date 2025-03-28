using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavedManager : MonoBehaviour
{
    // This class is responsible for keeping lists of saved puzzles for each difficulty in files and return them when queried. Can store/remove a single puzzle at a time or query if puzzle is saved.
    // File format: this class keeps folder "saved" in Application.persistentDataPath. Inside there's a file for each difficulty. One puzzle per line.
    // Asks DifficultyManager for difficulty every time it operates.

    // PUBLIC
    public static SavedManager Instance { get; private set; }
    public event EventHandler OnSaveUnsave;
    public void SaveUnsavePuzzle(string puzzle)
    {
        if (IsPuzzleSaved(puzzle))
        {
            string[] lines = File.ReadAllLines(GetFilePath());
            using StreamWriter writer = new(GetFilePath(), false);
            foreach (string line in lines)
            {
                if (line != puzzle) writer.WriteLine(line);
            }
        }
        else
        {
            using StreamWriter writer = new(GetFilePath(), true);
            writer.WriteLine(puzzle);
        }
        OnSaveUnsave?.Invoke(this, EventArgs.Empty);
    }
    public static bool IsPuzzleSaved(string puzzle)
    {
        return GetSavedPuzzles().Contains(puzzle);
    }
    public static List<string> GetSavedPuzzles()
    {
        if (File.Exists(GetFilePath()))
        {
            string[] lines = File.ReadAllLines(GetFilePath());
            return new List<string>(lines);
        }
        return new List<string>();
    }

    // PRIVATE
    private const string SAVED_PUZZLES_FOLDER_NAME = "saved";
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVED_PUZZLES_FOLDER_NAME));
    }
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVED_PUZZLES_FOLDER_NAME, DifficultyManager.Instance.CurrentDifficulty.DifficultyName);
    }
}
