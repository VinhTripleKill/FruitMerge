using UnityEngine;
using System.Collections;

public class WinSequenceController : MonoBehaviour
{
    public Animator ringAnimator;
    public Animator triangleAnimator;

    public float lifeTime = 1.3f;

    public void Play(Vector3 playerPos)
    {
        transform.position = playerPos;

        ringAnimator.Play("Ring_ExpandShrink", 0, 0f);
        triangleAnimator.Play("Triangle_FlyIn", 0, 0f);

        StartCoroutine(WinSequenceRoutine());
    }

    private IEnumerator WinSequenceRoutine()
    {
        // ⏳ Đợi animation chạy xong
        yield return new WaitForSeconds(lifeTime);

        // 🎉 Báo thắng SAU KHI animation xong
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTargetReached();
        }

        Destroy(gameObject);
    }
}
