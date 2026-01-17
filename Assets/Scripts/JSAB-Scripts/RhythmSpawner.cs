using UnityEngine;

public class RhythmSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public SpawnPoint[] spawnPoints;

    // ===== SIGNAL EVENTS =====

    public void SpawnAtIndex(int index)
    {
        if (index < 0 || index >= spawnPoints.Length) return;
        spawnPoints[index].Spawn();
    }

    public void SpawnRandom()
    {
        if (spawnPoints.Length == 0) return;
        spawnPoints[Random.Range(0, spawnPoints.Length)].Spawn();
    }
}
