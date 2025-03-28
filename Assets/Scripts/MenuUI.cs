using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button newBoardButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button savedButton;
    [SerializeField] private Button completedButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button difficultyButtonNext;
    [SerializeField] private Button difficultyButtonPrev;
    [SerializeField] private MenuPopupGroup menuPopupGroup;
    [SerializeField] private CompletedSavedUIGroup completedSavedUIGroup;
    [SerializeField] private DeferredHintDialog deferredHintDialog;
    [SerializeField] private Sprite saveButtonSprite;
    [SerializeField] private Sprite unsaveButtonSprite;
    private void Start()
    {
        newBoardButton.onClick.AddListener(() =>
        {
            CurrentPuzzleSaver.Instance.RemoveFile();
            BoardManager.Instance.LoadNewPuzzle();
        });
        hintButton.onClick.AddListener(() =>
        {
            if (PuzzleSolver.Instance.HintsAvailable) BoardManager.Instance.Hint();
            else deferredHintDialog.DeferHint();
        });
        saveButton.onClick.AddListener(() =>
        {
            CurrentPuzzleSaver.TryReadPuzzle(out string puzzle);
            SavedManager.Instance.SaveUnsavePuzzle(puzzle);
        });
        savedButton.onClick.AddListener(() => completedSavedUIGroup.Show(CompletedSavedUI.PuzzleSet.Saved));
        completedButton.onClick.AddListener(() => completedSavedUIGroup.Show(CompletedSavedUI.PuzzleSet.Completed));
        menuButton.onClick.AddListener(() => menuPopupGroup.Show());
        resetButton.onClick.AddListener(() =>
        {
            while (CurrentPuzzleSaver.Instance.IsUndoAvailable) BoardManager.Instance.Undo();

            // this approach is cleaner but begins hint calculation anew
            // CurrentPuzzleSaver.Instance.TryReadPuzzle(out string puzzle);
            // BoardManager.Instance.LoadNewPuzzle(puzzle);
        });
        undoButton.onClick.AddListener(() => BoardManager.Instance.Undo());
        difficultyButton.onClick.AddListener(() => DifficultyManager.Instance.SwitchDifficulty());
        difficultyButtonNext.onClick.AddListener(() => DifficultyManager.Instance.SwitchDifficulty());
        difficultyButtonPrev.onClick.AddListener(() => DifficultyManager.Instance.PreviousDifficulty());
        CurrentPuzzleSaver.Instance.OnUndoStateChange += (_, _) =>
        {
            undoButton.interactable = CurrentPuzzleSaver.Instance.IsUndoAvailable;
            resetButton.interactable = CurrentPuzzleSaver.Instance.IsUndoAvailable;
        };
        DifficultyManager.Instance.OnDifficultyChange += (_, _) => UpdateDifficultyButtonText();
        UpdateDifficultyButtonText();
        SavedManager.Instance.OnSaveUnsave += (_, _) => UpdateSaveButtonSprite();
        BoardManager.Instance.OnPuzzleLoaded += (_, _) => UpdateSaveButtonSprite();
        UpdateSaveButtonSprite();
    }
    private void UpdateDifficultyButtonText()
    {
        difficultyButton.GetComponentInChildren<TextMeshProUGUI>().text = DifficultyManager.Instance.CurrentDifficulty.DifficultyName;
    }
    private void UpdateSaveButtonSprite()
    {
        saveButton.GetComponentInChildren<Image>().sprite = CurrentPuzzleSaver.TryReadPuzzle(out string puzzle) && SavedManager.IsPuzzleSaved(puzzle)
            ? unsaveButtonSprite
            : saveButtonSprite;
    }
}
