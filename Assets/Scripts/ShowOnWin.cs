using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ShowOnWin : MonoBehaviour
{
    /*
    If puzzle is loaded with some moves, this calls RefreshVisibility multiple times anyway
    private void Start()
    {
        BoardManager.Instance.OnPuzzleLoaded += (_, _) => RefreshVisibility();
        BoardManager.Instance.OnMove += (_, _) => RefreshVisibility();
    }
    */
    private void Update()
    {
        RefreshVisibility();
    }
    private void RefreshVisibility()
    {
        GetComponent<Image>().enabled = PuzzleSolver.Instance.IsWin();
    }
}
