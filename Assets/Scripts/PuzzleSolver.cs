using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{
    // This class understands the puzzle, but only the single puzzle at hand.
    // This is the only class to know Obstacle, but doesn't know history, doesn't know about difficulties or puzzle strings.
    // It can solve puzzles (offer hints). It receives from BoardManager obstacles to spawn and performed steps (by user, undo or load retracing).
    // Doesn't care about remembering states - new puzzle comes, forget the old one, it's others' job to remember, I'm the solver.
    // Careful: its definition of Move is local and may differ from other definitions.
    // Communicates using primitives (int x, y, bool isVertical, etc.) to avoid confusion.
    // Usage: call NewBoard(), then AddObstacle(...) for each obstacle, then FinishedAddingObstacles().
    // Register moves with MakeMove(...), for hint use SuggestMove(...) when HintsAvailable == true, note it won't register move automatically.

    // PUBLIC
    public static PuzzleSolver Instance { get; private set; }
    public bool HintsAvailable { get; private set; }
    public void NewBoard()
    {
        // Debug.Log("puzzle solver initializing new board");
        if (boardTreeCalculation != null) StopCoroutine(boardTreeCalculation);
        currentState = new();
    }
    public void AddObstacle(int x, int y, bool isVertical, int length, bool isTarget = false)
    {
        // Debug.Log("adding obstacle to puzzle solver: " + x + " " + y + " " + length + " " + (isVertical ? "vertical" : "horizontal"));
        currentState.AddObstacleWithoutChecking(new Obstacle(new Vector2Int(x, y), isVertical ? Vector2Int.up : Vector2Int.right, length, isTarget));
    }
    public void FinishedAddingObstacles()
    {
        // Debug.Log("puzzle solver starting hint calculating");
        // start calculating hints
        HintsAvailable = false;
        boardTreeCalculation = CalculateBoardTree();
        StartCoroutine(boardTreeCalculation);
    }
    public void MakeMove(int x1, int y1, int x2, int y2) // register user input / undo / retracing
    {
        // Debug.Log("adding move to puzzle solver: " + x1 + " " + y1 + " " + x2 + " " + y2);
        /*
        if (x1 == x2 && y1 == y2)
        {
            Debug.Log("PuzzleSolver receiving move in place (probably from winning condition)");
            return;
        }
        */
        foreach (Move move in currentState.GetMoves())
        {
            // Debug.Log("trying " + move.oldObstacle.Position.x + " " + move.oldObstacle.Position.y + " " + move.newObstacle.Position.x + " " + move.newObstacle.Position.y);
            if (move.OldObstacle.Position.x == x1
                && move.OldObstacle.Position.y == y1
                && move.NewObstacle.Position.x == x2
                && move.NewObstacle.Position.y == y2)
            {
                currentState = move.NewState;
                return;
            }
        }
        Debug.LogError("wrong move");
    }
    public void SuggestMove(out int x1, out int y1, out int x2, out int y2) // for hint; doesn't make the move, this is done by MakeMove()
    {
        // assume hints are ready
        foreach (Move move in currentState.GetMoves())
        {
            if (movesToWin[move.NewState] == movesToWin[currentState] - 1)
            {
                x1 = move.OldObstacle.Position.x;
                y1 = move.OldObstacle.Position.y;
                x2 = move.NewObstacle.Position.x;
                y2 = move.NewObstacle.Position.y;
                Debug.Log(movesToWin[currentState] + " moves to go");
                return;
            }
        }
        Debug.LogWarning("PuzzleSolver can't find move to suggest as hint (winning condition might be met already)");
        x1 = 0;
        y1 = 0;
        x2 = 0;
        y2 = 0;
    }
    public bool IsWin()
    {
        return currentState.IsWin();
    }

    // PRIVATE
    private void Awake()
    {
        Instance = this;
    }

    private class Obstacle : IEquatable<Obstacle>
    {
        public Vector2Int Position { get; private set; }
        public Vector2Int Orientation { get; private set; } // Vector2Int.right or Vector2Int.up
        public int Length { get; }
        public bool IsTarget { get; }
        public Obstacle(Vector2Int position, Vector2Int orientation, int length, bool isTarget = false)
        {
            Position = position;
            Orientation = orientation;
            Length = length;
            IsTarget = isTarget;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Position.x, Position.y, Orientation.x, Orientation.y, Length, IsTarget);
            // hash collisions do happen: HashCode.Combine(new Vector2Int(4, 4)) == HashCode.Combine(new Vector2Int(0, 5))
        }
        public bool Equals(Obstacle other)
        {
            return GetHashCode() == other.GetHashCode();
        }
        public override string ToString()
        {
            string orientationString = Orientation == Vector2Int.right ? "horizontal" : "vertical";
            string targetString = IsTarget ? " target" : string.Empty;
            return orientationString + " " + Position + " " + Length + targetString;
        }
    }
    private readonly struct Move
    {
        public readonly Obstacle OldObstacle;
        public readonly Obstacle NewObstacle;
        public readonly BoardState NewState;
        public Move(Obstacle oldObstacle, Obstacle newObstacle, BoardState newState)
        {
            OldObstacle = oldObstacle;
            NewObstacle = newObstacle;
            NewState = newState;
        }
    }
    private class BoardState : IEquatable<BoardState>
    {
        private const int BOARD_SIZE = 6;
        private readonly List<Obstacle> obstacles;
        private readonly bool[,] grid;
        private int hashSum;
        public BoardState()
        {
            obstacles = new();
            grid = new bool[BOARD_SIZE, BOARD_SIZE];
            hashSum = 0;
        }
        public void AddObstacleWithoutChecking(Obstacle obstacle)
        {
            // don't check collisions, just update grid
            if (obstacle.Orientation == Vector2Int.right)
            {
                for (int i = 0; i < obstacle.Length; i++) grid[obstacle.Position.x + i, obstacle.Position.y] = true;
            }
            else
            {
                for (int i = 0; i < obstacle.Length; i++) grid[obstacle.Position.x, obstacle.Position.y + i] = true;
            }
            obstacles.Add(obstacle);
            hashSum += obstacle.GetHashCode(); // not perfect
        }
        private bool TryAddObstacle(Obstacle obstacle)
        {
            // check if it fits on the board
            if (obstacle.Position.x < 0) return false;
            if (obstacle.Position.y < 0) return false;
            if (obstacle.Orientation == Vector2Int.right && obstacle.Position.x + obstacle.Length > BOARD_SIZE) return false;
            if (obstacle.Orientation == Vector2Int.up && obstacle.Position.y + obstacle.Length > BOARD_SIZE) return false;

            // check collisions and update grid
            if (obstacle.Orientation == Vector2Int.right)
            {
                for (int i = 0; i < obstacle.Length; i++) if (grid[obstacle.Position.x + i, obstacle.Position.y]) return false;
                for (int i = 0; i < obstacle.Length; i++) grid[obstacle.Position.x + i, obstacle.Position.y] = true;
            }
            else
            {
                for (int i = 0; i < obstacle.Length; i++) if (grid[obstacle.Position.x, obstacle.Position.y + i]) return false;
                for (int i = 0; i < obstacle.Length; i++) grid[obstacle.Position.x, obstacle.Position.y + i] = true;
            }

            obstacles.Add(obstacle);
            hashSum += obstacle.GetHashCode(); // not perfect
            return true;
        }
        private BoardState DeepCopyWithout(Obstacle movingObstacle)
        {
            BoardState result = new();
            foreach (Obstacle obstacle in obstacles)
            {
                if (!movingObstacle.Equals(obstacle)) result.AddObstacleWithoutChecking(obstacle);
            }
            return result;
        }
        public List<Move> GetMoves()
        {
            List<Move> result = new();
            foreach (Obstacle movingObstacle in obstacles)
            {
                if (movingObstacle.Length == 1) continue; // not moving after all
                Vector2Int moveVector = Vector2Int.zero;
                while (true)
                {
                    BoardState newState = DeepCopyWithout(movingObstacle);
                    moveVector += movingObstacle.Orientation;
                    Obstacle movedObstacle = new(movingObstacle.Position + moveVector, movingObstacle.Orientation, movingObstacle.Length, movingObstacle.IsTarget);
                    if (!newState.TryAddObstacle(movedObstacle)) break;
                    result.Add(new Move(movingObstacle, movedObstacle, newState));
                }
                moveVector = Vector2Int.zero;
                while (true)
                {
                    BoardState newState = DeepCopyWithout(movingObstacle);
                    moveVector -= movingObstacle.Orientation;
                    Obstacle movedObstacle = new(movingObstacle.Position + moveVector, movingObstacle.Orientation, movingObstacle.Length, movingObstacle.IsTarget);
                    if (!newState.TryAddObstacle(movedObstacle)) break;
                    result.Add(new Move(movingObstacle, movedObstacle, newState));
                }
            }
            return result;
        }
        public bool IsWin()
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.IsTarget) return obstacle.Position.x == 4;
            }
            // Debug.LogError("target obstacle not found"); //probably checking on new() BoardState
            return false;
        }
        public override int GetHashCode()
        {
            return hashSum;
        }
        public bool Equals(BoardState other)
        {
            return GetHashCode() == other.GetHashCode();
        }
        public override string ToString()
        {
            string result = string.Empty;
            foreach (Obstacle obstacle in obstacles)
            {
                result += obstacle.ToString();
                result += "; ";
            }
            return result;
        }
    }
    private BoardState currentState;
    private Dictionary<BoardState, int> movesToWin;
    private IEnumerator boardTreeCalculation;
    private IEnumerator CalculateBoardTree()
    {
        const float allowed_calculation_time_chunk = 0.012f; // 12ms per frame (60 FPS is 16.(6) ms per frame)
        float calculationChunkBeginTime = Time.realtimeSinceStartup;

        HashSet<BoardState> possibleStates = new() { currentState };
        HashSet<BoardState> possibleWinStates = new();
        Queue<BoardState> statesToProcess = new();
        statesToProcess.Enqueue(currentState);
        while (statesToProcess.Count > 0)
        {
            BoardState processedState = statesToProcess.Dequeue();
            if (processedState.IsWin()) possibleWinStates.Add(processedState);
            foreach (Move move in processedState.GetMoves())
            {
                BoardState state = move.NewState;
                if (!possibleStates.Contains(state))
                {
                    statesToProcess.Enqueue(state);
                    possibleStates.Add(state);
                }
            }

            if (Time.realtimeSinceStartup - calculationChunkBeginTime > allowed_calculation_time_chunk)
            {
                yield return null;
                calculationChunkBeginTime = Time.realtimeSinceStartup;
            }
        }

        movesToWin = new();
        foreach (BoardState state in possibleWinStates)
        {
            statesToProcess.Enqueue(state);
            movesToWin[state] = 0;
        }
        while (statesToProcess.Count > 0)
        {
            BoardState processedState = statesToProcess.Dequeue();
            int movesToNextState = movesToWin[processedState] + 1;
            foreach (Move move in processedState.GetMoves())
            {
                BoardState state = move.NewState;
                if (!movesToWin.ContainsKey(state) || movesToWin[state] > movesToNextState)
                {
                    movesToWin[state] = movesToNextState;
                    statesToProcess.Enqueue(state);
                }
            }

            if (Time.realtimeSinceStartup - calculationChunkBeginTime > allowed_calculation_time_chunk)
            {
                yield return null;
                calculationChunkBeginTime = Time.realtimeSinceStartup;
            }
        }
        // Debug.Log("solution has " + movesToWin[currentState] + " moves");
        // Debug.Log("found " + possibleStates.Count + " possible states");
        HintsAvailable = true;
    }
}
