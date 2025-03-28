using System;
using UnityEngine;

public class SmallObstacle : MonoBehaviour
{
    // PUBLIC
    public static event EventHandler OnAnySmallObstacleSet;
    public static void ResetStaticData()
    {
        OnAnySmallObstacleSet = null;
    }
    public void SetObstacle(int posX, int posY, int length, bool isVertical, bool isTarget, float cellSize, float gapSize, float offset)
    {
        this.posX = posX;
        this.posY = posY;
        this.length = length;
        this.isVertical = isVertical;
        IsTarget = isTarget;
        this.cellSize = cellSize;
        this.gapSize = gapSize;
        for (int i = 0; i < BOARD_SIZE; i++) grid[i] = i * cellSize + (i + 1) * gapSize + offset;
        RefreshPosition();
        OnAnySmallObstacleSet?.Invoke(this, EventArgs.Empty);
    }
    public bool IsTarget { get; private set; }
    public bool IsBarrier()
    {
        return length == 1;
    }

    // PRIVATE
    private const int BOARD_SIZE = 6;
    private float cellSize;
    private float gapSize;
    private readonly float[] grid = new float[BOARD_SIZE];
    private int posX, posY;
    private int length;
    private bool isVertical;
    private void RefreshPosition()
    {
        GetComponent<RectTransform>().sizeDelta = isVertical
            ? new Vector2(cellSize, length * cellSize + (length - 1) * gapSize)
            : new Vector2(length * cellSize + (length - 1) * gapSize, cellSize);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(grid[posX], -grid[posY]);
    }
}
