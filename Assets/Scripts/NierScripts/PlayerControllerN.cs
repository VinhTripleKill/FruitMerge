using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerN : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public Transform firePos;
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float rayLength = 100f;
    [Header("Move Particle")]
    [SerializeField] private Transform moveParticle;
    [SerializeField] private float rotateLerpSpeed = 10f;
    private ParticleSystem movePS;

    private Rigidbody2D rb;
    private Vector2 movement;

    // Dash state
    private bool isDashing = false;
    private bool canDash = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movePS = moveParticle.GetComponent<ParticleSystem>();

        movePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Update()
    {
        if (!isDashing)
        {
            float movex = Input.GetAxisRaw("Horizontal");
            float movey = Input.GetAxisRaw("Vertical");
            movement = new Vector2(movex, movey).normalized;
        }

        UpdateMoveParticleRotation();

        FollowDirectionCursorMouse();
        DrawAimRay();

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }


    private void UpdateMoveParticleRotation()
    {
        // KHÔNG di chuyển → tắt particle
        if (movement == Vector2.zero)
        {
            if (movePS.isPlaying)
                movePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            return;
        }

        // CÓ di chuyển → bật particle
        if (!movePS.isPlaying)
            movePS.Play();

        // movement.y -> rotation.x
        float targetRotX = movement.y * 90f;

        // movement.x -> rotation.y
        float targetRotY = -movement.x * 90f;

        Quaternion targetRot = Quaternion.Euler(
            targetRotX,
            targetRotY,
            0f
        );

        moveParticle.localRotation = Quaternion.Lerp(
            moveParticle.localRotation,
            targetRot,
            Time.deltaTime * rotateLerpSpeed
        );
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }
    
    private void FollowDirectionCursorMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90f;
    }
    
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        Vector2 dashDir;

        // Nếu có input di chuyển → dash theo input
        if (movement != Vector2.zero)
        {
            dashDir = movement.normalized;
        }
        else
        {
            // Không có input → dash theo hướng đang nhìn
            dashDir = firePos.up.normalized;
        }

        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

   
    private void DrawAimRay()
    {
        Vector2 origin = transform.position;
        Vector2 direction = firePos.position - transform.position;
        Debug.DrawRay(origin, direction.normalized * rayLength, Color.red);
    }
}
