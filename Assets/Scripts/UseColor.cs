using UnityEngine;
using UnityEngine.UI;

public class UseColor : MonoBehaviour
{
    // Recognized by ThemeManager.
    public enum PaletteChoice
    {
        BackgroundColor,
        BoardColor,
        ButtonColor,
        DisabledButtonColor,
        ObstacleColor,
        TargetColor,
        BarrierColor,
        TextColor,
        AccentColor,
    }

    public PaletteChoice ColorChoice;

    // For Button's Animator.
    public bool IsButtonDisabled { get; private set; }
    public bool IsButtonAccented { get; private set; }

    public void RecolorButtonDisabled()
    {
        IsButtonDisabled = true;
        if (ColorChoice == PaletteChoice.ButtonColor) GetComponent<Image>().color = ThemeManager.Instance.CurrentTheme.DisabledButtonColor;
    }
    public void RecolorButtonEnabled()
    {
        IsButtonDisabled = false;
        if (ColorChoice == PaletteChoice.ButtonColor) GetComponent<Image>().color = IsButtonAccented ? ThemeManager.Instance.CurrentTheme.AccentColor : ThemeManager.Instance.CurrentTheme.ButtonColor;
    }
    public void RecolorButtonAccented()
    {
        IsButtonAccented = true;
        if (ColorChoice == PaletteChoice.ButtonColor) GetComponent<Image>().color = ThemeManager.Instance.CurrentTheme.AccentColor;
    }
    public void RecolorButtonUnaccented()
    {
        IsButtonAccented = false;
        if (ColorChoice == PaletteChoice.ButtonColor) GetComponent<Image>().color = IsButtonDisabled ? ThemeManager.Instance.CurrentTheme.DisabledButtonColor : ThemeManager.Instance.CurrentTheme.ButtonColor;
    }
}
