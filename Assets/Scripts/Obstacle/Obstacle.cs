using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody2D rb;
    public float minspeed = 5f;
    public float maxspeed = 150f;
    public float minSclae = 0.5f;
    public float maxScale = 2.0f;
    public float minTorque=10;
    public float maxTorque=50;
    void Start()
    {
        float scale = Random.Range(minSclae, maxScale);
        transform.localScale = new Vector3(scale, scale, 1);
        rb = GetComponent<Rigidbody2D>();
        float speed = Random.Range(minspeed, maxspeed)/scale;
        Vector2 direction = Random.insideUnitCircle.normalized;
        rb.AddForce(direction * speed);
        float torque = Random.Range(minTorque, maxTorque);
        rb.AddTorque(torque);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
