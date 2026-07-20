using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FruitManager fruitManager;
    [SerializeField] private GameObject scorePopupCanvasPrefab;
    [Header("Merge Settings")]
    [SerializeField] private float destroyDelay = 0.05f;
    private void SpawnScorePopup(Vector3 worldPos, int score, int fruitID)
    {
    if (scorePopupCanvasPrefab == null) return;

    GameObject popupObj = Instantiate(scorePopupCanvasPrefab, worldPos, Quaternion.identity);

    ScorePopup popup = popupObj.GetComponent<ScorePopup>();

    if (popup != null)
    {
        popup.Setup(score, fruitID);
    }
    }
    public void Merge(Fruit fruitA, Fruit fruitB)
    {
        // Null check
        if (fruitA == null || fruitB == null) return;

        // Tránh merge nhiều lần
        if (fruitA.IsMerged || fruitB.IsMerged) return;

        // Khác loại thì bỏ qua
        if (fruitA.FruitID != fruitB.FruitID) return;

        int currentID = fruitA.FruitID;

        // Đánh dấu đã merge
        fruitA.SetMerged(true);
        fruitB.SetMerged(true);
        
        Vector3 mergePos = (fruitA.transform.position + fruitB.transform.position) * 0.5f;

        AudioManager.Instance.PlayMergeSound();
         int nextID = currentID + 1;
        
        ScoreManager.Instance.AddScore(fruitA.ScoreMerge);
        SpawnScorePopup(mergePos, fruitA.ScoreMerge, nextID);
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
       
        if (nextID >= fruitManager.GetMaxFruitID())
        {
            Debug.Log("Final Fruit Created!");

            GameManager.Instance.GameWin();
        }

        GameObject nextFruitPrefab = fruitManager.GetFruitPrefab(nextID);

        if (nextFruitPrefab != null)
        {
            GameObject newFruit = Instantiate( nextFruitPrefab, mergePos, Quaternion.identity);

            // Gán ID
            Fruit fruit = newFruit.GetComponent<Fruit>();

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