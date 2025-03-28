using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // This class loads puzzles from strings, i.e. sends them to PuzzleSolver, CurrentPuzzleSaver, BoardInterface.
    // Asks CurrentPuzzleSaver beforehand if there's a puzzle begun already.
    // Can take command to load puzzle from specific string (for completed/saved puzzles).
    // Receives moves from ObstacleInterfaces with MakeMove() and relays them to PuzzleSolver and CurrentPuzzleSaver.
    // Performs undo, but has no memory, relies on CurrentPuzzleSaver instead.

    // PUBLIC
    public static BoardManager Instance { get; private set; }
    public event EventHandler OnPuzzleLoaded;
    public event EventHandler OnMove;
    public void LoadNewPuzzle()
    {
        if (CurrentPuzzleSaver.Instance.TryLoadState(out string puzzle, out List<Move> moves))
        {
            LoadNewPuzzle(puzzle, false);
            foreach (Move move in moves)
            {
                // Debug.Log("restoring state, move " + move.x1 + " " + move.y1 + " " + move.x2 + " " + move.y2);
                MakeMove(move, false);
            }
        }
        else
        {
            List<string> puzzles = new(DifficultyManager.Instance.CurrentDifficulty.Puzzles.text.Split(NEWLINE_IN_PUZZLE_TEXT_ASSETS));
            if (puzzles.Count == CompletedManager.GetCompletedPuzzles().Count)
            {
                Debug.Log("completed all the puzzles, loading any");
            }
            else
            {
                foreach (string completedPuzzle in CompletedManager.GetCompletedPuzzles()) puzzles.Remove(completedPuzzle);
            }
            LoadNewPuzzle(puzzles[UnityEngine.Random.Range(0, puzzles.Count)]);
        }
    }
    public void LoadNewPuzzle(string puzzle, bool shouldStoreToFile = true)
    {
        if (shouldStoreToFile) CurrentPuzzleSaver.Instance.InitFile(puzzle);

        PuzzleSolver.Instance.NewBoard();
        BoardInterface.NewBoard();
        const int BOARD_SIZE = 6;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            int firstOccurence = puzzle.IndexOf(c.ToString());
            if (firstOccurence != -1)
            {
                int length = 0;
                foreach (char d in puzzle) if (d == c) length++;
                PuzzleSolver.Instance.AddObstacle(
                    firstOccurence % BOARD_SIZE,
                    firstOccurence / BOARD_SIZE,
                    puzzle.IndexOf(c.ToString() + c.ToString()) == -1,
                    length,
                    c == 'A');
                BoardInterface.SpawnObstacle(
                    firstOccurence % BOARD_SIZE,
                    firstOccurence / BOARD_SIZE,
                    puzzle.IndexOf(c.ToString() + c.ToString()) == -1,
                    length,
                    c == 'A');
            }
        }

        // unmovable obstacles
        for (int i = 0; i < BOARD_SIZE * BOARD_SIZE; i++)
        {
            if (puzzle[i] == 'x')
            {
                PuzzleSolver.Instance.AddObstacle(
                    i % BOARD_SIZE,
                    i / BOARD_SIZE,
                    false,
                    1);
                BoardInterface.SpawnObstacle(
                    i % BOARD_SIZE,
                    i / BOARD_SIZE,
                    false,
                    1);
            }
        }

        PuzzleSolver.Instance.FinishedAddingObstacles();
        OnPuzzleLoaded?.Invoke(this, EventArgs.Empty);
    }
    public void MakeMove(Move move, bool shouldStoreToFile = true)
    {
        if (move.x1 == move.x2 && move.y1 == move.y2)
        {
            Debug.Log("trying to make move in place (probably win condition already met)");
            return;
        }
        PuzzleSolver.Instance.MakeMove(move.x1, move.y1, move.x2, move.y2);
        if (shouldStoreToFile) CurrentPuzzleSaver.Instance.AddMove(move);

        foreach (ObstacleInterface obstacleInterface in FindObjectsByType<ObstacleInterface>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (obstacleInterface.GetPositionX() == move.x1 && obstacleInterface.GetPositionY() == move.y1)
            {
                obstacleInterface.MoveObstacle(move.x2, move.y2); // no break - there are move BoardInterfaces
            }
        }

        OnMove?.Invoke(this, EventArgs.Empty);
    }
    public static void FinishBoard()
    {
        CurrentPuzzleSaver.TryReadPuzzle(out string puzzle);
        CompletedManager.RememberPuzzle(puzzle);
    }
    public readonly struct Move
    {
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
        public readonly int x1;
        public readonly int y1;
        public readonly int x2;
        public readonly int y2;
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
        public Move(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public bool IsWinningMove()
        {
            return y1 == 2 && y2 == 2 && x2 == 4;
        }
    }
    public void Undo()
    {
        // assume it's available
        MakeMove(ReverseMove(CurrentPuzzleSaver.Instance.ReadAndRemoveLastMove()), false);
    }
    public void Hint()
    {
        // assume it's available
        PuzzleSolver.Instance.SuggestMove(out int x1, out int y1, out int x2, out int y2);
        Move move = new(x1, y1, x2, y2);
        if (!move.IsWinningMove()) MakeMove(move);
    }

    // PRIVATE
    private const string NEWLINE_IN_PUZZLE_TEXT_ASSETS = "\r\n";
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        LoadNewPuzzle();
        DifficultyManager.Instance.OnDifficultyChange += (_, _) => LoadNewPuzzle();
    }
    private static Move ReverseMove(Move move)
    {
        return new Move(move.x2, move.y2, move.x1, move.y1);
    }
}
