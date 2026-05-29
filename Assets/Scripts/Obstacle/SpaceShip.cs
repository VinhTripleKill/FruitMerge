using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SpaceShip : MonoBehaviour
{
    public float thrust = 10f;
    private Rigidbody2D rb;
    public float speed = 10f;
    public GameObject shipTail;
    private float score = 0;
    private float elapsedTime = 0f;
    public float scoreRate = 10f;
    public UIDocument uiDocument;
    private Label scoreText;
    private Button startButton;
    public GameObject  explosionEffect;
    public InputAction lookPosition;
    public InputAction thrustAction;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lookPosition.Enable();
        thrustAction.Enable();
    }   
    void Start()
    {
        shipTail.SetActive(false);
        scoreText = uiDocument.rootVisualElement.Q<Label>("Score");
        startButton = uiDocument.rootVisualElement.Q<Button>("StartButton");
        startButton.style.display= DisplayStyle.None;
        startButton.clicked += RestartGame;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreRate);
        scoreText.text = "Score: " + score.ToString();

        if (thrustAction.ReadValue<float>() > 0) 
        {
            Vector3 mousePosition = lookPosition.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = (worldPosition - transform.position).normalized;
            transform.up = direction;
            rb.AddForce(direction * thrust);
            shipTail.SetActive(true);
            if (rb.linearVelocity.magnitude > speed) { 
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }
        else if (thrustAction.ReadValue<float>() == 0) { 
            shipTail.SetActive(false);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);   
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            startButton.style.display= DisplayStyle.Flex;
        }
    }
    void RestartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
