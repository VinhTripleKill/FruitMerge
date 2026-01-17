using UnityEngine;
using UnityEngine.Playables;

public class CheckpointReceiver : MonoBehaviour
{
    public void SetCheckpoint()
    {
        GameManager.Instance.SaveCheckpoint();
    }
}
