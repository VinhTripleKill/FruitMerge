using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThrowController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float padding = 0.3f;

    [Header("Limits")]
    [SerializeField] private Transform leftLimit;

    [SerializeField] private Transform rightLimit;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform bottomFruitBox;

    [Header("Mobile Controls")]
    [SerializeField] private Button moveLeftButton;

    [SerializeField] private Button moveRightButton;

    [SerializeField] private Button throwButton;

    [SerializeField] private FruitSpawner fruitSpawner;

    private PlayerInput playerInput;

    private InputAction moveAction;

    private InputAction pauseAction;

    private InputAction clickAction;

    private float mobileInputX;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction =
            playerInput.actions["Move"];

        pauseAction =
            playerInput.actions["PauseGame"];
        clickAction =
    playerInput.actions["Click"];
    }

    private void Update()
    {
        Move();

        DrawLine();

        PauseInputCheck();

        ClickInputCheck();
    }
    private void ClickInputCheck()
    {
        if (clickAction.WasPressedThisFrame())
        {
            AudioManager.Instance.PlayClickSound();
        }
    }
    private void Move()
    {
        if (GameManager.Instance.IsPaused)
            return;

        Vector2 input =
            moveAction.ReadValue<Vector2>();

        float finalInputX =
            input.x + mobileInputX;

        Vector3 pos = transform.position;

        pos.x +=
            finalInputX *
            moveSpeed *
            Time.deltaTime;

        pos.x = Mathf.Clamp(
            pos.x,
            leftLimit.position.x + padding,
            rightLimit.position.x - padding
        );

        transform.position = pos;
    }

    private void DrawLine()
    {
        if (lineRenderer == null ||
            bottomFruitBox == null)
            return;

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(
            0,
            transform.position
        );

        lineRenderer.SetPosition(
            1,
            new Vector3(
                transform.position.x,
                bottomFruitBox.position.y,
                transform.position.z
            )
        );
    }

    private void PauseInputCheck()
    {
        if (GameManager.Instance.GameplayLocked)
            return;

        if (pauseAction.WasPressedThisFrame())
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void MobileMoveLeftDown()
    {
        mobileInputX = -1f;
    }

    public void MobileMoveRightDown()
    {
        mobileInputX = 1f;
    }

    public void MobileMoveUp()
    {
        mobileInputX = 0f;
    }

    // ========= MOBILE THROW =========

    public void MobileThrow()
    {
        if (fruitSpawner != null)
        {
            fruitSpawner.MobileThrow();
        }
    }
    public void ResetPosition()
    {
        float centerX =
            (leftLimit.position.x +
             rightLimit.position.x) / 2f;

        Vector3 pos = transform.position;

        pos.x = centerX;

        transform.position = pos;
    }
    public void TeleportToX(float targetX)
    {
        float clampedX = Mathf.Clamp(
            targetX,
            leftLimit.position.x + padding,
            rightLimit.position.x - padding
        );

        Vector3 pos = transform.position;

        pos.x = clampedX;

        transform.position = pos;
    }
}