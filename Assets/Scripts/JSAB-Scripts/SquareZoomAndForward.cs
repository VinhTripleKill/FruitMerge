using UnityEngine;

public class SquareZoomAndForward : MonoBehaviour
{
    [Header("Scale Settings")]
    public float zoomDuration = 0.5f;

    [Header("Movement Settings")]
    public float moveSpeed = 20f;

    [Header("Life Time")]
    public float lifeTime = 5f;

    private Vector3 targetScale;
    private float zoomTimer = 0f;
    private bool canMove = false;

    void Start()
    {
        // Lưu scale ban đầu
        targetScale = transform.localScale;

        // Scale về 0
        transform.localScale = Vector3.zero;

        // Auto destroy
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        HandleZoom();
        HandleMove();
    }

    void HandleZoom()
    {
        if (zoomTimer < zoomDuration)
        {
            zoomTimer += Time.deltaTime;
            float t = zoomTimer / zoomDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            if (zoomTimer >= zoomDuration)
            {
                transform.localScale = targetScale;
                canMove = true;
            }
        }
    }

    void HandleMove()
    {
        if (!canMove) return;

        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
}
