using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour, IPointerDownHandler
{
    private const string PLAYER_PREFS_LAST_WIDTH = "LastWidth";
    private const string PLAYER_PREFS_LAST_HEIGHT = "LastHeight";
    private const int DEFAULT_WIDTH = 1200;
    private const int DEFAULT_HEIGHT = 960;
    [SerializeField] private Sprite fullScreenToggleSprite;
    [SerializeField] private Sprite windowedToggleSprite;
    [SerializeField] private bool changeParentSpacingTo400OnHide;

    private int _lastWidth;
    private int LastWidth
    {
        get => _lastWidth;
        set
        {
            _lastWidth = value;
            PlayerPrefs.SetInt(PLAYER_PREFS_LAST_WIDTH, LastWidth);
        }
    }

    private int _lastHeight;
    private int LastHeight
    {
        get => _lastHeight;
        set
        {
            _lastHeight = value;
            PlayerPrefs.SetInt(PLAYER_PREFS_LAST_HEIGHT, LastHeight);
        }
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_LAST_WIDTH))
        {
            LastWidth = PlayerPrefs.GetInt(PLAYER_PREFS_LAST_WIDTH);
            LastHeight = PlayerPrefs.GetInt(PLAYER_PREFS_LAST_HEIGHT);
        }
        else
        {
            LastWidth = DEFAULT_WIDTH;
            LastHeight = DEFAULT_HEIGHT;
        }
    }
    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            if (changeParentSpacingTo400OnHide) GetComponentInParent<HorizontalLayoutGroup>().spacing = 400;
            Destroy(gameObject);
        }
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            GetComponent<Button>().onClick.AddListener(() => SwitchFullscreen());
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) SwitchFullscreen();
    }
    private void Update()
    {
        GetComponentInChildren<Image>().sprite = Screen.fullScreen ? windowedToggleSprite : fullScreenToggleSprite;
    }
    private void SwitchFullscreen()
    {
        if (Screen.fullScreen) Windowed();
        else FullScreen();
    }
    private void Windowed()
    {
        Screen.SetResolution(LastWidth, LastHeight, false);
    }
    public void FullScreen()
    {
        LastWidth = Screen.width;
        LastHeight = Screen.height;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
