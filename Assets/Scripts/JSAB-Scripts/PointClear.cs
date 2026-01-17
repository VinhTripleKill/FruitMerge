using UnityEngine;

public class PointClear : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OnPointClearTouched();
            Destroy(gameObject);
        }
    }

}
