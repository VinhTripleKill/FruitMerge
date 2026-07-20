using UnityEngine;

public class ParticalEffect : MonoBehaviour
{
    public static ParticalEffect Instance;

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private ParticleSystem clickEffect;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void SpawnClickEffect(Vector2 screenPosition)
    {
        if (clickEffect == null || mainCamera == null)
            return;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -mainCamera.transform.position.z));

        worldPos.z = 0f;

        ParticleSystem effect =
            Instantiate(clickEffect, worldPos, Quaternion.identity);

        Destroy(effect.gameObject,
            effect.main.duration +
            effect.main.startLifetime.constantMax);
    }
}