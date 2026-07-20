using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FruitManager fruitManager;

    [SerializeField] private Transform throwPos;

    [Header("Spawn Settings")]
    [SerializeField] private float reloadTime = 0.5f;

    [SerializeField] private int minRandomFruitID = 0;

    [SerializeField] private int maxRandomFruitID = 2;

    private PlayerInput playerInput;

    private InputAction throwAction;

    private GameObject currentFruit;

    private bool canThrow = true;

    private int nextFruitID;
    [SerializeField] private MergeSystem mergeSystem;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        throwAction = playerInput.actions["Throw"];
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused) return;
        if (GameManager.Instance.GameplayLocked) return;
        FollowThrowPos();

        ThrowFruit();
    }

    private void FollowThrowPos()
    {
        if (currentFruit == null) return;

        currentFruit.transform.position = throwPos.position;
    }

    private void ThrowFruit()
    {
        if (GameManager.Instance.IsGameWin) return;

        if (GameManager.Instance.IsGameOver) return;

        if (!canThrow) return;

        if (!throwAction.WasPressedThisFrame()) return;

        PerformThrow();
    }
    private void GenerateNextFruit()
    {
        nextFruitID = Random.Range(minRandomFruitID,maxRandomFruitID + 1);

        // Update next fruit UI
        GameObject prefab = fruitManager.GetFruitPrefab(nextFruitID);

        if (prefab != null)
        {
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();

            if (sr != null && UIManager.Instance.NextFruitVisual != null)
            {
                UIManager.Instance.NextFruitVisual.sprite = sr.sprite;
            }
        }
    }


    private IEnumerator ReloadFruit()
    {
        canThrow = false;

        yield return new WaitForSeconds(reloadTime);

        SpawnHoldingFruit();

        canThrow = true;
    }


    private void SpawnHoldingFruit()
    {
        
        GameObject fruitPrefab = fruitManager.GetFruitPrefab(nextFruitID);

        if (fruitPrefab == null) return;

        currentFruit = Instantiate( fruitPrefab, throwPos.position, Quaternion.identity
        );

        // Gắn ID cho fruit
        Fruit fruit =currentFruit.GetComponent<Fruit>();
 
        if (fruit != null)
        {
            fruit.SetFruitID(nextFruitID);
        }

        // Fruit đang giữ sẽ chưa bị physics
        Rigidbody2D rb =currentFruit.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Parent vào ThrowPos
        currentFruit.transform.parent = throwPos;
        GenerateNextFruit();
    }
    public void MobileThrow()
    {
        if (GameManager.Instance.IsGameWin) return;

        if (GameManager.Instance.IsGameOver) return;

        if (!canThrow) return;

        PerformThrow();
    }
    private void PerformThrow()
    {
        if (currentFruit == null) return;

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
 
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }

        currentFruit.transform.parent = null;

        ScoreManager.Instance.AddThrowCount();

        currentFruit = null;

        StartCoroutine(ReloadFruit());
    }
    public void StartGameSpawn()
    {
        ClearAllFruits();

        GenerateNextFruit();

        SpawnHoldingFruit();
    }
    public void ClearAllFruits()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);

        foreach (Fruit fruit in allFruits)
        {
            Destroy(fruit.gameObject);
        }

        currentFruit = null;

        StopAllCoroutines();

        canThrow = true;
    }
}