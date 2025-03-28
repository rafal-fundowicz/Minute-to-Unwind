using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CurrentPuzzleSaver : MonoBehaviour
{
    // This class is responsible to keep puzzle states in files, so that user can switch difficulties, exit, then come back at any time and find everything intact.
    // File format: this class keeps folder "current" in Application.persistentDataPath. Inside there's a file for each difficulty.
    // First line is puzzle string, then 4 lines per move (x1, y1, x2, y2).
    // Asks DifficultyManager for difficulty every time it operates.

    // PUBLIC
    public static CurrentPuzzleSaver Instance;
    public event EventHandler OnUndoStateChange;
    private bool _isUndoAvailable;
    public bool IsUndoAvailable
    {
        get => _isUndoAvailable;
        private set
        {
            _isUndoAvailable = value;
            OnUndoStateChange?.Invoke(this, EventArgs.Empty);
        }
    }
    public void InitFile(string puzzle)
    {
        using StreamWriter writer = new(GetFilePath(), false);
        writer.WriteLine(puzzle);
        HistoryCount = 0;
    }
    public void RemoveFile()
    {
        string path = GetFilePath();
        if (File.Exists(path)) File.Delete(path);
        HistoryCount = 0;
    }
    public void AddMove(BoardManager.Move move)
    {
        if (File.Exists(GetFilePath()))
        {
            using StreamWriter writer = new(GetFilePath(), true);
            writer.WriteLine(move.x1);
            writer.WriteLine(move.y1);
            writer.WriteLine(move.x2);
            writer.WriteLine(move.y2);
            HistoryCount++;
        }
    }
    public BoardManager.Move ReadAndRemoveLastMove()
    {
        string[] lines = File.ReadAllLines(GetFilePath());
        BoardManager.Move move = new(
            int.Parse(lines[^4]),
            int.Parse(lines[^3]),
            int.Parse(lines[^2]),
            int.Parse(lines[^1]));
        File.WriteAllLines(GetFilePath(), lines[..^4]);
        HistoryCount--;
        return move;
    }
    public static bool TryReadPuzzle(out string puzzle)
    {
        if (!File.Exists(GetFilePath()))
        {
            puzzle = string.Empty;
            return false;
        }
        using StreamReader reader = new(GetFilePath());
        puzzle = reader.ReadLine();
        return true;
    }
    public bool TryLoadState(out string puzzle, out List<BoardManager.Move> moves)
    {
        if (!File.Exists(GetFilePath()))
        {
            puzzle = string.Empty;
            moves = null;
            return false;
        }
        using StreamReader reader = new(GetFilePath());
        puzzle = reader.ReadLine();
        moves = new();
        string input1, input2, input3, input4;
        BoardManager.Move move;
        HistoryCount = 0;
        while (!string.IsNullOrEmpty(input1 = reader.ReadLine()))
        {
            input2 = reader.ReadLine();
            input3 = reader.ReadLine();
            input4 = reader.ReadLine();
            move = new(
                int.Parse(input1),
                int.Parse(input2),
                int.Parse(input3),
                int.Parse(input4));
            moves.Add(move);
            HistoryCount++;
            // Debug.Log("puzzle saved read move " + input1 + " " + input2 + " " + input3 + " " + input4);
        }
        return true;
    }

    // PRIVATE
    private int _historyCount;
    private int HistoryCount
    {
        get => _historyCount;
        set
        {
            _historyCount = value;
            IsUndoAvailable = HistoryCount > 0;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private const string CURRENT_PUZZLES_FOLDER_NAME = "current";
    private void Start()
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, CURRENT_PUZZLES_FOLDER_NAME));
    }
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CURRENT_PUZZLES_FOLDER_NAME, DifficultyManager.Instance.CurrentDifficulty.DifficultyName);
    }
}
