using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Prefab To Spawn")]
    public GameObject prefab;

    public void Spawn()
    {
        if (prefab == null) return;

        Instantiate(prefab, transform.position, transform.rotation);
    }
}
