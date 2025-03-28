using UnityEngine;

[RequireComponent(typeof(UseColor))]
public class ButtonAccentColorOnWin : MonoBehaviour
{
    /*
    If puzzle is loaded with some moves, this calls RefreshColor multiple times anyway, and might set wrong color at the end
    private void Start()
    {
        BoardManager.Instance.OnPuzzleLoaded += (_, _) => RefreshColor();
        BoardManager.Instance.OnMove += (_, _) => RefreshColor();
    }
    */
    private void Update()
    {
        RefreshColor();
    }
    private void RefreshColor()
    {
        if (PuzzleSolver.Instance.IsWin()) GetComponent<UseColor>().RecolorButtonAccented();
        else GetComponent<UseColor>().RecolorButtonUnaccented();
    }
}
