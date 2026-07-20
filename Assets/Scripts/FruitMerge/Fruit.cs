using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit Info")]
    [SerializeField] private int fruitID;
    [SerializeField] private int scoreMerge = 10;

    public int ScoreMerge => scoreMerge;
    [SerializeField] private bool canMerge = true;
    private MergeSystem mergeSystem;
    private bool isMerged = false;

    

    public int FruitID => fruitID;

    public bool CanMerge => canMerge;

    public bool IsMerged => isMerged;
    private void Awake()
    {
        mergeSystem = FindAnyObjectByType<MergeSystem>();
    }
    public void SetFruitID(int id)
    {
        fruitID = id;
    }

    public void SetMerged(bool value)
    {
        isMerged = value;
    }

    private void OnCollisionEnter2D(
    Collision2D collision
)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            GameManager.Instance.GameOver();
        }

        // ===== MERGE =====

        if (!canMerge) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        if (otherFruit == null) return;

        if (otherFruit.FruitID != fruitID) return;

        if (otherFruit.IsMerged || isMerged) return;

        mergeSystem.Merge(this, otherFruit);
    }
}