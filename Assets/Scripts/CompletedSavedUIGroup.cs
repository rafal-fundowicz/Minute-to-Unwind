using UnityEngine;

public class CompletedSavedUIGroup : MonoBehaviour
{
    public void Show(CompletedSavedUI.PuzzleSet puzzleSet)
    {
        gameObject.SetActive(true);
        foreach(CompletedSavedUI completedSavedUI in FindObjectsByType<CompletedSavedUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            completedSavedUI.Load(puzzleSet);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
