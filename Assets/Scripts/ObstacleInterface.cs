using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObstacleInterface : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // PUBLIC
    public static event EventHandler OnWin;
    public static event EventHandler OnGrab;
    public static event EventHandler OnDrop;
    public static event EventHandler OnCollision;
    public static event EventHandler OnAnyObstacleSet;
    public static void ResetStaticData()
    {
        OnGrab = null;
        OnDrop = null;
        OnCollision = null;
        OnWin = null;
        OnAnyObstacleSet = null;
    }

    public void SetObstacle(int posX, int posY, int length, bool isVertical, bool isTarget, float cellSize, float gapSize, float offset, float roundness)
    {
        this.posX = posX;
        this.posY = posY;
        this.length = length;
        this.isVertical = isVertical;
        this.isTarget = isTarget;
        GetComponent<RectTransform>().sizeDelta = isVertical
            ? new Vector2(cellSize, length * cellSize + (length - 1) * gapSize)
            : new Vector2(length * cellSize + (length - 1) * gapSize, cellSize);
        GetComponent<Image>().pixelsPerUnitMultiplier = roundness;
        for (int i = 0; i < BOARD_SIZE; i++) grid[i] = i * cellSize + (i + 1) * gapSize + offset;
        RefreshPosition();
        OnAnyObstacleSet?.Invoke(this, EventArgs.Empty);
    }
    public void MoveObstacle(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        RefreshPosition();
    }
    public int GetPositionX()
    {
        return posX;
    }
    public int GetPositionY()
    {
        return posY;
    }
    public bool IsTarget()
    {
        return isTarget;
    }
    public bool IsBarrier()
    {
        return length == 1;
    }

    // DRAGGING
    private Vector2 startDragPosition;
    private Vector2 draggingAnchor;
    private float minBound, maxBound;
    private bool justCollidedMin;
    private bool justCollidedMax;
    private static bool isBoardLocked;
    private const int BOUNDS_LAYER = 10;
    private const int WINNING_POSITION_X = 4;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isBoardLocked || length == 1) return;
        OnGrab?.Invoke(this, EventArgs.Empty);
        GetComponent<Image>().raycastTarget = false;
        startDragPosition = transform.position;
        draggingAnchor = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) - (Vector2)transform.position;
        GetComponent<BoxCollider2D>().enabled = false;
        if (isVertical)
        {
            minBound = Physics2D.Raycast(transform.position, Vector2.down).collider.bounds.max.y + ScreenToWorldSize(GetComponent<BoxCollider2D>().size.y);
            maxBound = Physics2D.Raycast(transform.position, Vector2.up).collider.bounds.min.y;
        }
        else
        {
            minBound = Physics2D.Raycast(transform.position, Vector2.left).collider.bounds.max.x;
            maxBound = Physics2D.Raycast(transform.position, Vector2.right).collider.bounds.min.x - ScreenToWorldSize(GetComponent<BoxCollider2D>().size.x);

            if (isTarget)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.right);
                GetComponent<BoxCollider2D>().enabled = true;
                if (raycast.transform.gameObject.layer == BOUNDS_LAYER) maxBound = Mathf.Infinity;
            }
        }
        GetComponent<BoxCollider2D>().enabled = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isBoardLocked || length == 1) return;
        Vector2 dragPosition = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) - draggingAnchor;
        if (isVertical)
        {
            float newY = dragPosition.y;
            if (newY < minBound)
            {
                if (!justCollidedMin)
                {
                    justCollidedMin = true;
                    justCollidedMax = false;
                    OnCollision?.Invoke(this, EventArgs.Empty);
                }
                newY = minBound;
            }
            else if (newY > maxBound)
            {
                if (!justCollidedMax)
                {
                    justCollidedMax = true;
                    justCollidedMin = false;
                    OnCollision?.Invoke(this, EventArgs.Empty);
                }
                newY = maxBound;
            }
            else
            {
                justCollidedMin = false;
                justCollidedMax = false;
            }
            transform.position = new Vector2(startDragPosition.x, newY);
        }
        else
        {
            float newX = dragPosition.x;
            if (newX < minBound)
            {
                if (!justCollidedMin)
                {
                    justCollidedMin = true;
                    justCollidedMax = false;
                    OnCollision?.Invoke(this, EventArgs.Empty);
                }
                newX = minBound;
            }
            else if (newX > maxBound)
            {
                if (!justCollidedMax)
                {
                    justCollidedMax = true;
                    justCollidedMin = false;
                    OnCollision?.Invoke(this, EventArgs.Empty);
                }
                newX = maxBound;
            }
            else
            {
                justCollidedMin = false;
                justCollidedMax = false;
            }
            transform.position = new Vector2(newX, startDragPosition.y);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isBoardLocked || length == 1) return;

        OnDrop?.Invoke(this, EventArgs.Empty);

        RectTransform rectTransform = GetComponent<RectTransform>();
        float newFloatPositionX = RoundTo(rectTransform.anchoredPosition.x, grid, out int newPosX);
        float newFloatPositionY = -RoundTo(-rectTransform.anchoredPosition.y, grid, out int newPosY);
        if (isTarget && newPosX >= WINNING_POSITION_X)
        {
            UpdateLockedBoardState(true);
            OnWin?.Invoke(this, EventArgs.Empty);
            int oldPosX = posX;
            int oldPosY = posY;
            posX = WINNING_POSITION_X;
            posY = newPosY;
            BoardManager.Instance.MakeMove(new BoardManager.Move(oldPosX, oldPosY, posX, posY), true);
            BoardManager.FinishBoard();
            slideOutCoroutine = SlideOut();
            StartCoroutine(slideOutCoroutine);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector3(newFloatPositionX, newFloatPositionY);

            if (posX != newPosX || posY != newPosY)
            {
                // Debug.Log("ObstacleInterface sending move to BoardManager " + posX + " " + posY + " " + newPosX + " " + newPosY);
                BoardManager.Instance.MakeMove(new BoardManager.Move(posX, posY, newPosX, newPosY), true);
                posX = newPosX;
                posY = newPosY;
            }
        }

        GetComponent<Image>().raycastTarget = true;
    }

    // PRIVATE
    private const int BOARD_SIZE = 6;
    private const float SHADOW_SIZE = 3f;
    private readonly float[] grid = new float[BOARD_SIZE];
    private int posX, posY;
    private int length;
    private bool isVertical;
    private bool isTarget;
    private void Start()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<RectTransform>().sizeDelta.x + SHADOW_SIZE, GetComponent<RectTransform>().sizeDelta.y + SHADOW_SIZE);
        GetComponent<BoxCollider2D>().offset = new Vector2((GetComponent<RectTransform>().sizeDelta.x + SHADOW_SIZE) / 2, -(GetComponent<RectTransform>().sizeDelta.y + SHADOW_SIZE) / 2);
        BoardManager.Instance.OnPuzzleLoaded += (_, _) => UpdateLockedBoardState();
    }
    private static void UpdateLockedBoardState()
    {
        isBoardLocked = PuzzleSolver.Instance.IsWin();
    }
    private static void UpdateLockedBoardState(bool isLocked)
    {
        isBoardLocked = isLocked;
    }
    private void RefreshPosition()
    {
        UpdateLockedBoardState();
        GetComponent<RectTransform>().anchoredPosition = new Vector2(grid[posX], -grid[posY]);
        if (slideOutCoroutine != null) StopCoroutine(slideOutCoroutine);
        if (isTarget && posX >= WINNING_POSITION_X) transform.position = new(SLIDE_OUT_POSITION_X, transform.position.y);
    }
    private static float RoundTo(float value, float[] grid, out int gridNum)
    {
        float minDifference = Mathf.Infinity;
        int index = -1;
        for (int i = 0; i < grid.Length; i++)
        {
            float difference = Mathf.Abs(value - grid[i]);
            if (difference < minDifference)
            {
                minDifference = difference;
                index = i;
            }
        }
        gridNum = index;
        return grid[index];
    }
    private float ScreenToWorldSize(float size)
    {
        float scale;

        CanvasScaler canvasScaler = gameObject.GetComponentInParent<CanvasScaler>();

        if (canvasScaler.matchWidthOrHeight == 0f)
        {
            // match width
            scale = (float)Screen.width / (float)canvasScaler.referenceResolution.x;
        }
        else if (canvasScaler.matchWidthOrHeight == 1f)
        {
            // match height
            scale = (float)Screen.height / (float)canvasScaler.referenceResolution.y;
        }
        else
        {
            Debug.LogError("unsupported canvas scaling mode");
            scale = 1f;
        }

        return size * 2 * Camera.main.orthographicSize / Screen.height * scale;
    }
    private IEnumerator slideOutCoroutine;
    private const float SLIDE_OUT_POSITION_X = 100f;
    private const float SLIDE_OUT_SPEED = 10f;
    private IEnumerator SlideOut()
    {
        while (transform.position.x < SLIDE_OUT_POSITION_X)
        {
            transform.position += SLIDE_OUT_SPEED * Time.deltaTime * Vector3.right;
            yield return null;
        }
    }
}
