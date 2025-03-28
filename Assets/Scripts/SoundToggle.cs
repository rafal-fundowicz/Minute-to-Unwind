using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    private bool isOn;
    [SerializeField] private Sprite checkedSoundToggleSprite;
    [SerializeField] private Sprite uncheckedSoundToggleSprite;

    private void Start()
    {
        SoundManager.OnModifiedSoundEnabled += (_, _) => UpdateState();
        GetComponent<Button>().onClick.AddListener(() => SoundManager.Instance.IsSoundEnabled = !isOn);
        UpdateState();
    }
    private void UpdateState()
    {
        isOn = SoundManager.Instance.IsSoundEnabled;
        GetComponentInChildren<Image>().sprite = isOn ? checkedSoundToggleSprite : uncheckedSoundToggleSprite;
    }
}
