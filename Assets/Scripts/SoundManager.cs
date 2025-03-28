using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public static event EventHandler OnModifiedSoundEnabled;
    public static void ResetStaticData()
    {
        OnModifiedSoundEnabled = null;
    }
    private const string PLAYER_PREFS_SOUND_STATE = "SoundState";
    private bool _isSoundEnabled;
    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set
        {
            _isSoundEnabled = value;
            OnModifiedSoundEnabled?.Invoke(null, EventArgs.Empty);
            PlayerPrefs.SetInt(PLAYER_PREFS_SOUND_STATE, IsSoundEnabled ? 1 : 0);
        }
    }
    [SerializeField] private AudioClip grabClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip collisionClip;
    [SerializeField] private AudioClip applauseClip;
    private void Start()
    {
        if ((PlayerPrefs.HasKey(PLAYER_PREFS_SOUND_STATE) && PlayerPrefs.GetInt(PLAYER_PREFS_SOUND_STATE) == 1) || (!PlayerPrefs.HasKey(PLAYER_PREFS_SOUND_STATE)))
        {
            IsSoundEnabled = true;
        }
        ObstacleInterface.OnWin += ObstacleInterface_OnWin;
        ObstacleInterface.OnGrab += ObstacleInterface_OnGrab;
        ObstacleInterface.OnDrop += ObstacleInterface_OnDrop;
        ObstacleInterface.OnCollision += ObstacleInterface_OnCollision;
    }
    private void ObstacleInterface_OnWin(object sender, EventArgs e)
    {
        if (IsSoundEnabled) AudioSource.PlayClipAtPoint(applauseClip, Camera.main.transform.position);
    }
    private void ObstacleInterface_OnGrab(object sender, EventArgs e)
    {
        // if (SoundEnabled) AudioSource.PlayClipAtPoint(grabClip, Camera.main.transform.position);
    }
    private void ObstacleInterface_OnDrop(object sender, EventArgs e)
    {
        // if (SoundEnabled) AudioSource.PlayClipAtPoint(dropClip, Camera.main.transform.position);
    }
    private void ObstacleInterface_OnCollision(object sender, EventArgs e)
    {
        if (IsSoundEnabled) AudioSource.PlayClipAtPoint(collisionClip, Camera.main.transform.position);
    }
}
