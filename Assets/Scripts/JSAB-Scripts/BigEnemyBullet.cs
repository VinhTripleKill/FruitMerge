using UnityEngine;

public class BigEnemyBullets : MonoBehaviour
{
    [Header("Big Bullet Movement")]
    [SerializeField] private float speedBigBullet = 5f;
    [SerializeField] private float explodeDistance = 10f;

    [Header("Small Bullet Settings")]
    [SerializeField] private GameObject bulletEnemyPrefab;
    [SerializeField] private int bulletCount = 6;
    [SerializeField] private float speedEnemyBullet = 6f;

    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = transform.position;
    }


    void Update()
    {
        // 🚀 Bay theo hướng của chính nó (local up)
        transform.position += transform.up * speedBigBullet * Time.deltaTime;

        float traveled = Vector3.Distance(startPos, transform.position);
        if (traveled >= explodeDistance)
        {
            SpawnSmallBullets();
            Destroy(gameObject);
        }
    }

    private void SpawnSmallBullets()
    {
        if (bulletEnemyPrefab == null) return;

        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0f
            );

            GameObject bullet = Instantiate(
                bulletEnemyPrefab,
                transform.position,
                Quaternion.identity
            );

            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.DirectionBullet(dir.normalized * speedEnemyBullet);
            }
        }
    }
}
