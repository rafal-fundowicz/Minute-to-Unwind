using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    private bool isOn;
    [SerializeField] private Sprite checkedMusicToggleSprite;
    [SerializeField] private Sprite uncheckedMusicToggleSprite;

    public static bool WasClicked { get; private set; } // for WebGL

    private void Start()
    {
        MusicManager.OnModifiedMusicEnabled += (_, _) => UpdateState();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            WasClicked = true; // for WebGL
            MusicManager.Instance.IsMusicEnabled = !isOn;
        });
        UpdateState();
    }
    private void UpdateState()
    {
        isOn = MusicManager.Instance.IsMusicEnabled;
        GetComponentInChildren<Image>().sprite = isOn ? checkedMusicToggleSprite : uncheckedMusicToggleSprite;
    }
}
