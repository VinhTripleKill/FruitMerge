using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Ring Rotation")]
    [SerializeField] private GameObject ringInnerRotation;
    [SerializeField] private GameObject ringOuterRotation;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Win Sequence")]
    [SerializeField] private GameObject winSequencePrefab;   // ← Kéo prefab WinSequence vào đây

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool hasTriggeredWin = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 5f;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Target: Không tìm thấy Player!");
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null && !hasTriggeredWin)
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = dir * chaseSpeed;
        }
    }

    void Update()
    {
        if (!hasTriggeredWin)
        {
            RotateRings();
        }
    }

    private void RotateRings()
    {
        if (ringInnerRotation) ringInnerRotation.transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        if (ringOuterRotation) ringOuterRotation.transform.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggeredWin) return;

        if (other.CompareTag("Player"))
        {
            hasTriggeredWin = true;

            // 1. Dừng player (vẫn giữ)
            movePlayer playerScript = other.GetComponent<movePlayer>();
            if (playerScript != null)
            {
                playerScript.TriggerWinFreeze();
            }

            // 2. Spawn WinSequence tại vị trí Player
            if (winSequencePrefab != null && playerTransform != null)
            {
                GameObject winSeq = Instantiate(winSequencePrefab, playerTransform.position, Quaternion.identity);
                WinSequenceController controller = winSeq.GetComponent<WinSequenceController>();
                if (controller != null)
                {
                    controller.Play(playerTransform.position);
                }
            }

            // 3. Hủy Target
            Destroy(gameObject);

            // 4. Thông báo thắng (không pause game)
           
        }
    }
}