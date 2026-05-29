using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FruitManager fruitManager;

    [Header("Merge Settings")]
    [SerializeField] private float destroyDelay = 0.05f;
    public void Merge(Fruit fruitA, Fruit fruitB)
    {
        // Null check
        if (fruitA == null || fruitB == null)
            return;

        // Tránh merge nhiều lần
        if (fruitA.IsMerged || fruitB.IsMerged)
            return;

        // Khác loại thì bỏ qua
        if (fruitA.FruitID != fruitB.FruitID)
            return;

        int currentID = fruitA.FruitID;

        // Đánh dấu đã merge
        fruitA.SetMerged(true);
        fruitB.SetMerged(true);
        AudioManager.Instance.PlayMergeSound();
        // Add score
        ScoreManager.Instance.AddScore(
            fruitA.ScoreMerge
        );

        // Tính vị trí merge
        Vector3 mergePos =
            (fruitA.transform.position +
             fruitB.transform.position) / 2f;

        // Nếu là fruit cuối cùng
        if (currentID >= fruitManager.GetMaxFruitID())
        {
            Debug.Log("Final Fruit Merged!");

            // Infinity mode compatible:
            // chỉ destroy + cho điểm
            Destroy(fruitA.gameObject, destroyDelay);
            Destroy(fruitB.gameObject, destroyDelay);

            return;
        }

        // Spawn fruit mới
        int nextID = currentID + 1;

        if (nextID >= fruitManager.GetMaxFruitID())
        {
            Debug.Log("Final Fruit Created!");

            GameManager.Instance.GameWin();
        }

        GameObject nextFruitPrefab =
            fruitManager.GetFruitPrefab(nextID);

        if (nextFruitPrefab != null)
        {
            GameObject newFruit = Instantiate(
                nextFruitPrefab,
                mergePos,
                Quaternion.identity
            );

            // Gán ID
            Fruit fruit =
                newFruit.GetComponent<Fruit>();

            if (fruit != null)
            {
                fruit.SetFruitID(nextID);
            }
        }

        // Destroy fruit cũ
        Destroy(fruitA.gameObject, destroyDelay);
        Destroy(fruitB.gameObject, destroyDelay);
    }
}