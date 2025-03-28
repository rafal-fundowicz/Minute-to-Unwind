using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour
{
    private const string WEBSITE_URL = "https://fundowicz.com/";
    [SerializeField] private Button themeButton;
    [SerializeField] private Button themeButtonNext;
    [SerializeField] private Button themeButtonPrev;
    [SerializeField] private Button menuCloseButton;
    [SerializeField] private Button websiteButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private List<GameObject> destroyedInWebGL;
    [SerializeField] private float shrinkYInWebGL;
    private void Start()
    {
        themeButton.onClick.AddListener(() => ThemeManager.Instance.SwitchTheme());
        themeButtonNext.onClick.AddListener(() => ThemeManager.Instance.SwitchTheme());
        themeButtonPrev.onClick.AddListener(() => ThemeManager.Instance.PreviousTheme());
        ThemeManager.Instance.OnThemeChange += (_, _) => UpdateThemeText();
        UpdateThemeText();

        menuCloseButton.onClick.AddListener(() => GetComponentInParent<MenuPopupGroup>().Hide());

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            foreach (GameObject destroyable in destroyedInWebGL) Destroy(destroyable);
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - shrinkYInWebGL);
        }
        else
        {
            websiteButton.onClick.AddListener(() => Application.OpenURL(WEBSITE_URL));
            quitButton.onClick.AddListener(() => Quit());
        }
    }
    private void UpdateThemeText()
    {
        themeButton.GetComponentInChildren<TextMeshProUGUI>().text = ThemeManager.Instance.CurrentTheme.Name;
    }
    private static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
        }
        else
        {
            Application.Quit();
        }
    }
}
