using System.Collections.Generic;
using UnityEngine;

public class BoardInterface : MonoBehaviour
{
    // This class only handles spawning and despawning of visual obstacles for board loading. ObstacleInterfaces and BoardManager talk to each other by themselves.

    // PUBLIC
    public static void NewBoard()
    {
        foreach (ObstacleInterface obstacleInterface in FindObjectsByType<ObstacleInterface>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Destroy(obstacleInterface.gameObject);
        }
    }
    public static void SpawnObstacle(int x, int y, bool isVertical, int length, bool isTarget = false)
    {
        foreach (BoardInterface instance in Instances)
        {
            Transform newObstacleInterface = Instantiate(instance.obstacleInterfacePrefab, instance.transform);
            newObstacleInterface.GetComponent<ObstacleInterface>().SetObstacle(x, y, length, isVertical, isTarget, instance.cellSize, instance.gapSize, instance.offset, instance.roundness);
        }
    }

    // PRIVATE
    private static readonly List<BoardInterface> Instances = new();
    [SerializeField] private Transform obstacleInterfacePrefab;
    [SerializeField] private float cellSize;
    [SerializeField] private float gapSize;
    [SerializeField] private float offset;
    [SerializeField] private float roundness;
    private void Awake()
    {
        Instances.Add(this);
    }
}
