using UnityEngine;

public class CheckpointData
{
    public double timelineTime;
    public Vector3 playerSpawnPos;

    public CheckpointData(double time, Vector3 pos)
    {
        timelineTime = time;
        playerSpawnPos = pos;
    }
}
