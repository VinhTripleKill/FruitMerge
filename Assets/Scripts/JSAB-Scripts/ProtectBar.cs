using UnityEngine;
using UnityEngine.UI;

public class ProtectBar : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target;               // Player cần follow
    [SerializeField] private Vector3 offset; // nếu muốn lệch lên đầu player

    private bool isActive = false;
    private float indicateTimer = 0f;
    private float maxIndicatedTimer = 0f;
    private Image protectBarImage;

    void Awake()
    {
        protectBarImage = GetComponent<Image>();
        protectBarImage.fillAmount = 0f;
        isActive = false; // chỉ disable logic, KHÔNG disable GameObject
    }


    void LateUpdate()
    {
        // 🎯 FOLLOW PLAYER
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void Update()
    {
        if (!isActive) return;

        indicateTimer -= Time.deltaTime;
        protectBarImage.fillAmount = indicateTimer / maxIndicatedTimer;

        if (indicateTimer <= 0f)
        {
            StopProcessProtectBar();
        }
    }

    public void StartProcessProtectBar(float time)
    {
        gameObject.SetActive(true);
        isActive = true;
        maxIndicatedTimer = time;
        indicateTimer = time;
        protectBarImage.fillAmount = 1f;
    }

    public void StopProcessProtectBar()
    {
        isActive = false;
        indicateTimer = 0f;
        protectBarImage.fillAmount = 0f;
        gameObject.SetActive(false);
    }

    public bool IsActive() => isActive;
}
