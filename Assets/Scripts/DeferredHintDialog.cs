using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeferredHintDialog : MonoBehaviour
{
    [SerializeField] private List<Button> cancelButtons;
    private IEnumerator deferredHintCoroutine;
    private void Start()
    {
        foreach (Button cancelButton in cancelButtons)
        {
            cancelButton.onClick.AddListener(() =>
            {
                StopCoroutine(deferredHintCoroutine);
                Hide();
            });
        }
    }
    public void DeferHint()
    {
        Show();
        deferredHintCoroutine = DeferredHint();
        StartCoroutine(deferredHintCoroutine);
    }
    private IEnumerator DeferredHint()
    {
        while (!PuzzleSolver.Instance.HintsAvailable) yield return null;
        BoardManager.Instance.Hint();
        Hide();
    }
    private void Hide()
    {
        foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) canvas.enabled = false;
    }
    private void Show()
    {
        foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) canvas.enabled = true;
    }
}
