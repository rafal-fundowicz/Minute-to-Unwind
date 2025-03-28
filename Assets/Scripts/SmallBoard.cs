using UnityEngine;
using UnityEngine.UI;

public class SmallBoard : MonoBehaviour
{
    [SerializeField] private CompletedSavedUIGroup completedSavedUIGroup;
    [SerializeField] private Transform smallBoardVisual;
    [SerializeField] private Transform smallObstaclePrefab;
    [SerializeField] private float cellSize;
    [SerializeField] private float gapSize;
    [SerializeField] private float offset;
    private string puzzle;
    public void ShowNewBoard(string puzzle)
    {
        gameObject.SetActive(true);
        foreach (SmallObstacle smallObstacle in GetComponentsInChildren<SmallObstacle>())
        {
            Destroy(smallObstacle.gameObject);
        }
        const int BOARD_SIZE = 6;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            int firstOccurence = puzzle.IndexOf(c.ToString());
            if (firstOccurence != -1)
            {
                int length = 0;
                foreach (char d in puzzle) if (d == c) length++;
                SpawnObstacle(
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
                SpawnObstacle(
                    i % BOARD_SIZE,
                    i / BOARD_SIZE,
                    false,
                    1);
            }
        }
        this.puzzle = puzzle;
    }
    private void SpawnObstacle(int x, int y, bool isVertical, int length, bool isTarget = false)
    {
        Transform newSmallObstacle = Instantiate(smallObstaclePrefab, smallBoardVisual);
        newSmallObstacle.GetComponent<SmallObstacle>().SetObstacle(x, y, length, isVertical, isTarget, cellSize, gapSize, offset);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            BoardManager.Instance.LoadNewPuzzle(puzzle);
            completedSavedUIGroup.Hide();
        });
    }
}
