using UnityEngine;

public class SquareRoll : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 moveDir; // hướng bay CỐ ĐỊNH

    void Start()
    {
        moveDir = transform.up;   // chốt hướng bay ngay lúc spawn
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
