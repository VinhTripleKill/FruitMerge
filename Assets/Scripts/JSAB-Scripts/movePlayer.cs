 using UnityEngine;
using System.Collections;
public class movePlayer : MonoBehaviour
{
    [Header("Camera Limit")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float padding = 0.1f; // khoảng đệm an toàn
                                                   // Camera clamp bounds (dùng chung)
    private float minX, maxX, minY, maxY;
    private bool isWinFrozen = false;
    public GameObject[] pieces; // size = 4
    public int maxHP = 4;
    int hp;
    public float speed = 5f;
    [Header("Stretch Settings")]
    public float stretchAmount = 0.25f;
    public float stretchSpeed = 10f;
    public float rotateSpeed = 12f;
    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    [Header("Movement Trail Effects")]
    [SerializeField] private ParticleSystem moveUpEffect; // particle đuôi khi đi lên
    [SerializeField] private ParticleSystem moveDownEffect; // particle đuôi khi đi xuống
    [SerializeField] private ParticleSystem moveLeftEffect; // particle đuôi khi đi trái
    [SerializeField] private ParticleSystem moveRightEffect; // particle đuôi khi đi phải
    [Header("Dash Effect")]
    [SerializeField] private ParticleSystem dashParticle; // nếu bạn muốn dùng instance đã có sẵn trên Player
    [Header("Damage & Death")]
    [SerializeField] private ParticleSystem dieParticleEffect; // kéo Particle DieParticleEffect vào đây
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private ProtectBar protectBar;// thời gian cooldown giữa các lần nhận damage
    private bool canBeAttacked = true;
    private float damageCooldownTimer = 0f;
    private bool canTakeDamage = true;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 targetScale = Vector3.one;
    private float targetRotation = 0f;
    // Dash variables
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDirection;
    private float dashTimer;
    private float cooldownTimer;
    private bool wasMovingLastFrame = false; // theo dõi trạng thái di chuyển frame trước
    private Coroutine stretchRoutine = null; // để quản lý coroutine nhún
    // Để theo dõi particle đang bật (tránh bật/tắt liên tục mỗi frame)
    private ParticleSystem currentActiveEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = maxHP;
        UpdatePieces();
        if (mainCamera == null)
            mainCamera = Camera.main; // 🔥 FIX CHÍNH

        // Tắt hết particle lúc khởi động
        StopAllEffects();
        if (dashParticle != null)
        {
            dashParticle.transform.localPosition = Vector3.zero;
            dashParticle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        if (dieParticleEffect != null)
        {
            dieParticleEffect.transform.localPosition = Vector3.zero;
            dieParticleEffect.transform.localRotation = Quaternion.identity; // rotation.zero như bạn mô tả
            dieParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    private IEnumerator DashStretch()
    {
        transform.localScale = new Vector3(1.4f, 0.6f, 1);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }
    void Update()
    {
        if (isWinFrozen)
        {
            // Chỉ giữ nguyên trạng thái đã freeze, không xử lý input nữa
            return;
        }
        // Cooldown dash (giữ nguyên)
        if (!canDash)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canDash = true;
            }
        }
        if (!canTakeDamage)
        {
            damageCooldownTimer -= Time.deltaTime;
            if (damageCooldownTimer <= 0f)
            {
                canTakeDamage = true;
            }
        }
        // Nhận damage bằng phím K (có cooldown)
        if (canTakeDamage && Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage();
            canTakeDamage = false;
            damageCooldownTimer = damageCooldown; // reset cooldown 1 giây
        }
        if (canDash && !isDashing && Input.GetKeyDown(KeyCode.Space))
        {
            if (movement == Vector2.zero)
                movement = Vector2.up; // hướng mặc định

            StartDash();
        }

        if (isDashing)
        {
            HandleDash();
        }
        else
        {
            Move();
        }
        MoveAndChangeDirection();
        // ── Phần phát hiện vừa dừng di chuyển ───────────────────────────────
        bool isCurrentlyMoving = movement != Vector2.zero || isDashing;
        // Khi vừa chuyển từ MOVING → IDLE
        if (wasMovingLastFrame && !isCurrentlyMoving)
        {
            // Dừng coroutine cũ nếu đang chạy (tránh chồng)
            if (stretchRoutine != null)
                StopCoroutine(stretchRoutine);
            // Chạy 2 lần nhún liên tiếp
            stretchRoutine = StartCoroutine(PlayDoubleStretch());
        }
        wasMovingLastFrame = isCurrentlyMoving;
        // ────────────────────────────────────────────────────────────────────
    }
   
    public bool IsProtected()
    {
        if (protectBar == null) return false;

       
        return protectBar.IsActive(); 
        
    }
    public void TriggerWinFreeze()
    {
        isWinFrozen = true;
        StopAllEffects();
        if (dashParticle != null) dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        OriginShapes();

        // Tự động unfreeze sau 2 giây (nếu muốn)
        Invoke(nameof(UnfreezePlayer), 2f);
    }

    private void UnfreezePlayer()
    {
        isWinFrozen = false;
    }
    void LateUpdate()
    {
        ClampPlayerToCamera();
    }

    private void ClampPlayerToCamera()
    {
        if (mainCamera == null) return;

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camPos = mainCamera.transform.position;

        float halfWidth = 0.5f;
        float halfHeight = 0.5f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            halfWidth = col.bounds.extents.x;
            halfHeight = col.bounds.extents.y;
        }

        minX = camPos.x - camWidth + halfWidth + padding;
        maxX = camPos.x + camWidth - halfWidth - padding;
        minY = camPos.y - camHeight + halfHeight + padding;
        maxY = camPos.y + camHeight - halfHeight - padding;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }


    private IEnumerator PlayDoubleStretch()
    {

        yield return StartCoroutine(DashStretch());
        stretchRoutine = null;
    }
    private void Move()
    {
        if (isWinFrozen) return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;
        if (!isDashing)
        {
            rb.linearVelocity = movement * speed;
        }
    }
    private void StartDash()
    {
        if (isWinFrozen) return;
        isDashing = true;
        canDash = false;
        cooldownTimer = dashCooldown;
        dashDirection = movement.normalized;
        float dashSpeed = dashDistance / dashDuration;
        rb.linearVelocity = dashDirection * dashSpeed;
        dashTimer = dashDuration;
        canBeAttacked = false;
        if (dashParticle != null)
        {
            dashParticle.transform.localRotation = GetDashEffectRotation(dashDirection);
            dashParticle.Play();
        }
        // Tắt hết particle khi dash
        StopAllEffects();
    }
    private Quaternion GetDashEffectRotation(Vector2 direction)
    {
        // Chuyển vector thành góc (đơn vị độ)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Vì particle gốc hướng lên (rotation X = 90), ta xoay quanh trục Y
        // Các giá trị dưới đây dựa trên particle gốc hướng lên (0,0,1) trong local space
        if (direction == Vector2.zero) return Quaternion.Euler(90f, 0f, 0f);
        // 8 hướng chính + chéo
        // Góc tham chiếu: 0° = phải, 90° = lên, 180° = trái, -90°/270° = xuống
        // ------------------ Hướng chính ------------------
        if (Mathf.Abs(direction.y) < 0.1f)
        {
            if (direction.x > 0) return Quaternion.Euler(0f, -90f, 0f); // phải
            if (direction.x < 0) return Quaternion.Euler(0f, 90f, 0f); // trái
        }
        else if (Mathf.Abs(direction.x) < 0.1f)
        {
            if (direction.y > 0) return Quaternion.Euler(90f, 0f, 0f); // lên
            if (direction.y < 0) return Quaternion.Euler(-90f, 0f, 0f); // xuống
        }
        // ------------------ Hướng chéo ------------------
        else
        {
            // Góc thực tế → xoay quanh trục Y
            // Vì particle gốc hướng lên → ta cộng thêm 90° để "nằm ngang" rồi điều chỉnh
            return Quaternion.Euler(0f, -angle, 0f);
        }
        // fallback
        return Quaternion.Euler(90f, 0f, 0f);
    }
    private void HandleDash()
    {
        dashTimer -= Time.deltaTime;

        ClampPlayerToCamera(); // 🔥 ép clamp ngay khi dash

        if (dashTimer <= 0f)
        {
            isDashing = false;
            rb.linearVelocity = movement * speed;
            canBeAttacked = true;

            if (dashParticle != null)
                dashParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }


    private void MoveAndChangeDirection()
    {
        if (movement == Vector2.zero && !isDashing)
        {
            OriginShapes();
            StopAllEffects();
        }
        else
        {
            Vector2 dirForAnim = isDashing ? dashDirection : movement;
            ParticleSystem desiredEffect = null;
            if (dirForAnim.y > 0)
            {
                MoveUp();
                desiredEffect = moveUpEffect;
            }
            else if (dirForAnim.y < 0)
            {
                MoveDown();
                desiredEffect = moveDownEffect;
            }
            else if (dirForAnim.x < 0)
            {
                MoveLeft();
                desiredEffect = moveLeftEffect;
            }
            else if (dirForAnim.x > 0)
            {
                MoveRight();
                desiredEffect = moveRightEffect;
            }
            // Chỉ bật particle khi KHÔNG dash và có hiệu ứng mong muốn
            if (!isDashing)
            {
                SwitchEffect(desiredEffect);
            }
            else
            {
                StopAllEffects();
            }
        }
        // Lerp rotation & scale (luôn chạy)
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, targetRotation),
            Time.deltaTime * rotateSpeed
        );
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * stretchSpeed
        );
    }
    // ------------------- Particle Helper -------------------
    private void SwitchEffect(ParticleSystem targetEffect)
    {
        if (currentActiveEffect == targetEffect)
            return; // đã đúng rồi, không cần làm gì
        StopAllEffects();
        if (targetEffect != null)
        {
            targetEffect.Play();
            currentActiveEffect = targetEffect;
        }
    }
    public void SetProtectBar(ProtectBar bar)
    {
        protectBar = bar;
    }

    private void StopAllEffects()
    {
        if (moveUpEffect) moveUpEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (moveDownEffect) moveDownEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (moveLeftEffect) moveLeftEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (moveRightEffect) moveRightEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        currentActiveEffect = null;
    }
    // Các hàm Move... giữ nguyên như cũ
    private void MoveUp()
    {
        targetRotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
        targetScale = new Vector3(1f - stretchAmount, 1f + stretchAmount, 1f);
        if (Mathf.Abs(movement.x) == 0f)
            targetRotation = 0f;
    }
    private void MoveDown()
    {
        targetScale = new Vector3(1f - stretchAmount, 1f + stretchAmount, 1f);
        if (Mathf.Abs(movement.x) > 0f)
            targetRotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 90f;
        else
            targetRotation = 0f;
    }
    private void MoveLeft()
    {
        targetScale = new Vector3(1f + stretchAmount, 1f - stretchAmount, 1f);
        if (Mathf.Abs(movement.y) > 0f)
            targetRotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 180f;
        else
            targetRotation = 0f;
    }
    private void MoveRight()
    {
        targetScale = new Vector3(1f + stretchAmount, 1f - stretchAmount, 1f);
        if (Mathf.Abs(movement.y) > 0f)
            targetRotation = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        else
            targetRotation = 0f;
    }
    private void OriginShapes()
    {
        targetScale = Vector3.one;
        targetRotation = 0f;
    }

    void UpdatePieces()
    {
        for (int i = 0; i < pieces.Length; i++)
            pieces[i].SetActive(i < hp);
    }
    
    public void TakeDamage()
    {
        if (!canBeAttacked || hp <= 0) return;
        
        // Hiệu ứng flash (nếu có)
        TakeDamageEffect takeDamageEffect = GetComponent<TakeDamageEffect>();
        if (takeDamageEffect != null)
        {
            takeDamageEffect.CallDamageFlash();
        }

        hp--;
        UpdatePieces();

        // Gọi qua GameManager thay vì trực tiếp gọi ProtectBar
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerTakeDamage(damageCooldown);
        }

        if (hp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Phát particle chết
        if (dieParticleEffect != null)
        {
            // Đảm bảo particle đang ở vị trí player và rotation đúng
            dieParticleEffect.transform.SetParent(null); // tách ra khỏi player để không bị destroy theo
            dieParticleEffect.transform.position = transform.position;
            dieParticleEffect.transform.rotation = Quaternion.identity;
            dieParticleEffect.Play();
            // Tự động hủy particle sau khi hết thời gian sống (an toàn hơn)
            float lifetime = dieParticleEffect.main.startLifetime.constantMax + 0.5f;
            Destroy(dieParticleEffect.gameObject, lifetime);
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }

        // Optional: log hoặc gọi event game over
        Debug.Log("Player has died and been destroyed");
    }
}