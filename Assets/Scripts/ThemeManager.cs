using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    // PUBLIC
    public static ThemeManager Instance { get; private set; }
    public event EventHandler OnThemeChange;
    public ThemeSO CurrentTheme { get; private set; }
    public void SwitchTheme()
    {
        SetTheme((ThemeNumber + 1) % themeList.Count);
    }
    public void PreviousTheme()
    {
        if (ThemeNumber == 0) SetTheme(themeList.Count - 1);
        else SetTheme(ThemeNumber - 1);
    }

    // PRIVATE
    [SerializeField] private List<ThemeSO> themeList;
    private const string PLAYER_PREFS_THEME_NUMBER = "ThemeNumber";
    private int _themeNumber;
    private int ThemeNumber
    {
        get => _themeNumber;
        set
        {
            _themeNumber = value;
            PlayerPrefs.SetInt(PLAYER_PREFS_THEME_NUMBER, ThemeNumber);
        }
    }
    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_THEME_NUMBER))
        {
            int savedThemeNumber = PlayerPrefs.GetInt(PLAYER_PREFS_THEME_NUMBER);
            if (savedThemeNumber >= themeList.Count) SetTheme(0);
            else SetTheme(savedThemeNumber);
        }
        else
        {
            SetTheme(0);
        }
    }
    private void SetTheme(int themeNumber)
    {
        ThemeNumber = themeNumber;
        CurrentTheme = themeList[ThemeNumber];

        foreach (TextMeshProUGUI textMeshProUGUI in FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            textMeshProUGUI.color = CurrentTheme.TextColor;
        }
        foreach (ObstacleInterface obstacleInterface in FindObjectsByType<ObstacleInterface>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            obstacleInterface.gameObject.GetComponent<Image>().color = obstacleInterface.IsTarget() ? CurrentTheme.TargetColor : obstacleInterface.IsBarrier() ? CurrentTheme.BarrierColor : CurrentTheme.ObstacleColor;
        }
        foreach (SmallObstacle smallObstacle in FindObjectsByType<SmallObstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            smallObstacle.gameObject.GetComponent<Image>().color = smallObstacle.IsTarget ? CurrentTheme.TargetColor : smallObstacle.IsBarrier() ? CurrentTheme.BarrierColor : CurrentTheme.ObstacleColor;
        }
        foreach (UseColor useColor in FindObjectsByType<UseColor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            switch (useColor.ColorChoice)
            {
                case UseColor.PaletteChoice.BackgroundColor:
                    if (useColor.gameObject.TryGetComponent(out Image image)) image.color = CurrentTheme.BackgroundColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.BackgroundColor;
                    break;
                case UseColor.PaletteChoice.BoardColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.BoardColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.BoardColor;
                    break;
                case UseColor.PaletteChoice.ButtonColor:
                    Color currentButtonColor =
                        useColor.IsButtonDisabled ? CurrentTheme.DisabledButtonColor : useColor.IsButtonAccented ? CurrentTheme.AccentColor : CurrentTheme.ButtonColor;
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = currentButtonColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = currentButtonColor;
                    break;
                case UseColor.PaletteChoice.ObstacleColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.ObstacleColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.ObstacleColor;
                    break;
                case UseColor.PaletteChoice.TargetColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.TargetColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.TargetColor;
                    break;
                case UseColor.PaletteChoice.BarrierColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.BarrierColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.BarrierColor;
                    break;
                case UseColor.PaletteChoice.TextColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.TextColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.TextColor;
                    break;
                case UseColor.PaletteChoice.AccentColor:
                    if (useColor.gameObject.TryGetComponent(out image)) image.color = CurrentTheme.AccentColor;
                    else if (useColor.gameObject.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.color = CurrentTheme.AccentColor;
                    break;
            }
        }
        OnThemeChange?.Invoke(this, EventArgs.Empty);
    }
    private void Start()
    {
        ObstacleInterface.OnAnyObstacleSet += (sender, e) =>
        {
            ObstacleInterface obstacleInterface = sender as ObstacleInterface;
            obstacleInterface.gameObject.GetComponent<Image>().color = obstacleInterface.IsTarget() ? CurrentTheme.TargetColor : obstacleInterface.IsBarrier() ? CurrentTheme.BarrierColor : CurrentTheme.ObstacleColor;
        };
        SmallObstacle.OnAnySmallObstacleSet += (sender, e) =>
        {
            SmallObstacle smallObstacle = sender as SmallObstacle;
            smallObstacle.gameObject.GetComponent<Image>().color = smallObstacle.IsTarget ? CurrentTheme.TargetColor : smallObstacle.IsBarrier() ? CurrentTheme.BarrierColor : CurrentTheme.ObstacleColor;
        };
    }
}