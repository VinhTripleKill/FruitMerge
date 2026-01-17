using System.Collections;
using UnityEngine;

public class ShortLazerShoot : MonoBehaviour
{
    [Header("Laser References")]
    [SerializeField] private LineRenderer lazerShootWarning;
    [SerializeField] private LineRenderer lazerShoot;

    [Header("Collider for Damage")]
    [SerializeField] private BoxCollider2D damageCollider;

    [Header("Laser Settings")]
    [SerializeField] private float laserLength = 10f;
    [SerializeField] private float laserWidth = 0.2f;

    [Header("Timing")]
    [SerializeField] private float warningTime = 1f;
    [SerializeField] private float blinkTime = 0.2f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float shootTime = 0.2f;

    private void OnEnable()
    {
        if (damageCollider != null)
            damageCollider.enabled = false;

        StartCoroutine(LazerShootRoutine());
    }

    private IEnumerator LazerShootRoutine()
    {
        lazerShoot.enabled = false;
        if (damageCollider != null) damageCollider.enabled = false;

        // Warning
        SetupLaser(lazerShootWarning, false);
        lazerShootWarning.enabled = true;

        yield return new WaitForSeconds(warningTime);

        for (int i = 0; i < blinkCount; i++)
        {
            lazerShootWarning.enabled = false;
            yield return new WaitForSeconds(blinkTime);
            lazerShootWarning.enabled = true;
            yield return new WaitForSeconds(blinkTime);
        }

        // Shoot
        lazerShootWarning.enabled = false;
        SetupLaser(lazerShoot, true);
        lazerShoot.enabled = true;

        if (damageCollider != null)
            damageCollider.enabled = true;

        yield return new WaitForSeconds(shootTime);

        lazerShoot.enabled = false;
        if (damageCollider != null) damageCollider.enabled = false;

        // 💥 HỦY OBJECT SAU KHI BẮN XONG
        Destroy(gameObject);

    }

    private void SetupLaser(LineRenderer line, bool setupCollider)
    {
        // LineRenderer dùng LOCAL SPACE → xoay theo cha
        line.useWorldSpace = false;

        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.up * laserLength);

        line.startWidth = laserWidth;
        line.endWidth = laserWidth;

        // Collider kế thừa rotation từ LaserRoot
        if (setupCollider && damageCollider != null)
        {
            damageCollider.size = new Vector2(laserWidth, laserLength);
            damageCollider.offset = new Vector2(0f, laserLength / 2f);
            // ❌ KHÔNG set rotation ở đây
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.position;
        Vector2 dir = transform.up;
        Vector2 end = origin + dir.normalized * laserLength;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, end);
    }
}
