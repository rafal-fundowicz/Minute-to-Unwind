using System;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // PUBLIC
    public static DifficultyManager Instance { get; private set; }
    public event EventHandler OnDifficultyChange;
    public DifficultySO CurrentDifficulty { get; private set; }
    public void SwitchDifficulty()
    {
        SetDifficulty((DifficultyNumber + 1) % difficultyList.Count);
    }
    public void PreviousDifficulty()
    {
        if (DifficultyNumber == 0) SetDifficulty(difficultyList.Count - 1);
        else SetDifficulty(DifficultyNumber - 1);
    }

    // PRIVATE
    [SerializeField] private List<DifficultySO> difficultyList;
    private const string PLAYER_PREFS_DIFFICULTY_NUMBER = "DifficultyNumber";
    private int _difficultyNumber;
    private int DifficultyNumber
    {
        get => _difficultyNumber;
        set
        {
            _difficultyNumber = value;
            PlayerPrefs.SetInt(PLAYER_PREFS_DIFFICULTY_NUMBER, DifficultyNumber);
        }
    }
    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_DIFFICULTY_NUMBER))
        {
            int savedDifficultyNumber = PlayerPrefs.GetInt(PLAYER_PREFS_DIFFICULTY_NUMBER);
            if (savedDifficultyNumber >= difficultyList.Count) SetDifficulty(0);
            else SetDifficulty(savedDifficultyNumber);
        }
        else
        {
            SetDifficulty(0);
        }
    }
    private void SetDifficulty(int difficultyNumber)
    {
        DifficultyNumber = difficultyNumber;
        CurrentDifficulty = difficultyList[DifficultyNumber];
        OnDifficultyChange?.Invoke(this, EventArgs.Empty);
    }
}