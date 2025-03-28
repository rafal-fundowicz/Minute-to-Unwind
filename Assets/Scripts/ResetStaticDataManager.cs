using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        ObstacleInterface.ResetStaticData();
        SmallObstacle.ResetStaticData();
        MusicManager.ResetStaticData();
        SoundManager.ResetStaticData();
        AnyButton.ResetStaticData();
    }
}
