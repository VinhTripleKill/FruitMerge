using UnityEngine;

public class Bullets : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float timeDestroy = 2f;
    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
}

