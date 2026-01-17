using UnityEngine;

public class DamageOnTouchPlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageCooldown = 0.8f;     // thời gian giữa 2 lần damage (giây)

    private float nextDamageTime = 0f;                        // timestamp được damage tiếp theo

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextDamageTime) return;               // chưa hết cooldown thì bỏ quaa

        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<movePlayer>();
        if (player == null) return;

        // Nếu đang được bảo vệ thì không trừ máu
        if (player.IsProtected()) return;

        // Gây damage
        player.TakeDamage();

        // Cập nhật thời gian cho lần damage tiếp theo
        nextDamageTime = Time.time + damageCooldown;
    }

    // Optional: reset khi rời vùng trigger (không bắt buộc)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nextDamageTime = 0f; // cho phép damage ngay lần vào lại
        }
    }
}