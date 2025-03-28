using UnityEngine;

public class DisableChildrenDependingOnAspectRatio : MonoBehaviour
{
    [SerializeField] private bool hideWhenTallerThan;
    [SerializeField] private float widthMinBreakingPoint;
    [SerializeField] private float heightMinBreakingPoint;
    [SerializeField] private bool hideWhenWiderThan;
    [SerializeField] private float widthMaxBreakingPoint;
    [SerializeField] private float heightMaxBreakingPoint;
    private float minAspectRatio;
    private float maxAspectRatio;
    private int lastWidth;
    private int lastHeight;
    private void Awake()
    {
        minAspectRatio = widthMinBreakingPoint / heightMinBreakingPoint;
        maxAspectRatio = widthMaxBreakingPoint / heightMaxBreakingPoint;
    }
    private void Update() // Yuk! Unity has no callback for resolution change and OnRectTransformDimensionsChange doesn't always trigger
    {
        if (lastWidth != Screen.width || lastHeight != Screen.height) RefreshHideStatus();
    }
    private void RefreshHideStatus()
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        if ((hideWhenTallerThan && aspectRatio < minAspectRatio) || (hideWhenWiderThan && aspectRatio >= maxAspectRatio)) Hide();
        else Show();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
    private void Hide()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
    }
    private void Show()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(true);
    }
}
