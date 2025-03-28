using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletedSavedUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button invisibleCloseButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button difficultyButtonNext;
    [SerializeField] private Button difficultyButtonPrev;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<SmallBoard> smallBoards;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private string completedOnText;
    [SerializeField] private string savedOnText;
    private List<string> puzzles;
    private int maxPage;
    private int _currentPage;
    private int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            prevButton.gameObject.SetActive(CurrentPage != 0);
            nextButton.gameObject.SetActive(CurrentPage != maxPage);
            int i = 0;
            foreach (SmallBoard smallBoard in smallBoards)
            {
                if (CurrentPage * smallBoards.Count + i < puzzles.Count) smallBoard.ShowNewBoard(puzzles[CurrentPage * smallBoards.Count + i]);
                else smallBoard.Hide();
                i++;
            }
        }
    }
    private void Start()
    {
        closeButton.onClick.AddListener(() => GetComponentInParent<CompletedSavedUIGroup>().Hide());
        invisibleCloseButton.onClick.AddListener(() => GetComponentInParent<CompletedSavedUIGroup>().Hide());
        difficultyButton.onClick.AddListener(() => DifficultyManager.Instance.SwitchDifficulty());
        difficultyButtonNext.onClick.AddListener(() => DifficultyManager.Instance.SwitchDifficulty());
        difficultyButtonPrev.onClick.AddListener(() => DifficultyManager.Instance.PreviousDifficulty());
        DifficultyManager.Instance.OnDifficultyChange += (_, _) => Refresh();
        prevButton.onClick.AddListener(() =>
        {
            foreach(CompletedSavedUI completedSavedUI in FindObjectsByType<CompletedSavedUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                completedSavedUI.PrevPage();
            }
        });
        nextButton.onClick.AddListener(() =>
        {
            foreach(CompletedSavedUI completedSavedUI in FindObjectsByType<CompletedSavedUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                completedSavedUI.NextPage();
            }
        });
    }
    public void PrevPage()
    {
        CurrentPage--;
    }
    public void NextPage()
    {
        CurrentPage++;
    }
    private void Refresh()
    {
        difficultyButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyManager.Instance.CurrentDifficulty.DifficultyName;
        if (puzzleSet == PuzzleSet.Completed)
        {
            puzzles = CompletedManager.GetCompletedPuzzles(); // automatically reads current difficulty
            maxPage = (puzzles.Count - 1) / smallBoards.Count;
            mainText.text = puzzles.Count + completedOnText;
            CurrentPage = maxPage;
        }
        else
        {
            puzzles = SavedManager.GetSavedPuzzles(); // automatically reads current difficulty
            maxPage = (puzzles.Count - 1) / smallBoards.Count;
            mainText.text = puzzles.Count + savedOnText;
            CurrentPage = maxPage;
        }
    }
    public enum PuzzleSet
    {
        Completed,
        Saved,
    }
    private PuzzleSet puzzleSet;
    public void Load(PuzzleSet puzzleSet)
    {
        this.puzzleSet = puzzleSet;
        GetComponent<Canvas>().enabled = true;
        Refresh();
    }
}
