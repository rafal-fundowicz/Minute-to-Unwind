using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private const string PLAYER_PREFS_MUSIC_STATE = "MusicState";
    private void Start()
    {
        if ((PlayerPrefs.HasKey(PLAYER_PREFS_MUSIC_STATE) && PlayerPrefs.GetInt(PLAYER_PREFS_MUSIC_STATE) == 1) || (!PlayerPrefs.HasKey(PLAYER_PREFS_MUSIC_STATE)))
        {
            IsMusicEnabled = true;
        }
    }
    public static event EventHandler OnModifiedMusicEnabled;
    public static void ResetStaticData()
    {
        OnModifiedMusicEnabled = null;
    }
    private bool _isMusicEnabled;
    public bool IsMusicEnabled
    {
        get => _isMusicEnabled;
        set
        {
            _isMusicEnabled = value;
            OnModifiedMusicEnabled?.Invoke(null, EventArgs.Empty);
            PlayerPrefs.SetInt(PLAYER_PREFS_MUSIC_STATE, IsMusicEnabled ? 1 : 0);
            if (IsMusicEnabled)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer || MusicToggle.WasClicked)
                {
                    StartMusic();
                }
                else
                {
                    // We'd StartMusic() right now, but we're on WebGL without first user input yet. We'll enable music on first possible moment.
                    ObstacleInterface.OnGrab += AcknowledgeWebGLInput;
                    AnyButton.OnAnyButtonClick += AcknowledgeWebGLInput;
                }
            }
            else
            {
                StopMusic();
            }
        }
    }
    private void AcknowledgeWebGLInput(object sender, EventArgs e)
    {
        ObstacleInterface.OnGrab -= AcknowledgeWebGLInput;
        AnyButton.OnAnyButtonClick -= AcknowledgeWebGLInput;
        StartMusic();
    }
    private void StartMusic()
    {
        GetComponent<AudioSource>().enabled = true;
    }
    private void StopMusic()
    {
        GetComponent<AudioSource>().enabled = false;
    }
}
