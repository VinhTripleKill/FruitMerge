using System.Collections;
using UnityEngine;

public class FireWork : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject warningExplore;
    [SerializeField] private GameObject explore;

    [Header("Timing")]
    [SerializeField] private float warningTime = 2f;
    [SerializeField] private float exploreTime = 2f;

    [Header("Scale Effect")]
    [SerializeField] private float scaleDuration = 0.2f;

    [Header("Warning Rotate")]
    [SerializeField] private float rotateSpeed = 180f; // độ / giây

    private Vector3 warningOriginScale;
    private Vector3 exploreOriginScale;

    private void Start()
    {
        // Lưu scale gốc
        warningOriginScale = warningExplore.transform.localScale;
        exploreOriginScale = explore.transform.localScale;

        // Reset trạng thái ban đầu
        warningExplore.SetActive(false);
        explore.SetActive(false);

        StartCoroutine(FireWorkRoutine());
    }

    private void Update()
    {
        // warning xoay khi đang active
        if (warningExplore.activeSelf)
        {
            warningExplore.transform.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
        }
    }

    private IEnumerator FireWorkRoutine()
    {
        // ===== WARNING =====
        warningExplore.SetActive(true);
        yield return StartCoroutine(ScaleObject(warningExplore, Vector3.zero, warningOriginScale));

        yield return new WaitForSeconds(warningTime);

        yield return StartCoroutine(ScaleObject(warningExplore, warningOriginScale, Vector3.zero));
        warningExplore.SetActive(false);

        // ===== EXPLORE =====
        explore.SetActive(true);
        yield return StartCoroutine(ScaleObject(explore, Vector3.zero, exploreOriginScale));

        yield return new WaitForSeconds(exploreTime);

        yield return StartCoroutine(ScaleObject(explore, exploreOriginScale, Vector3.zero));
        explore.SetActive(false);

        // Destroy object cha
        Destroy(gameObject);
    }

    private IEnumerator ScaleObject(GameObject obj, Vector3 from, Vector3 to)
    {
        float t = 0f;
        obj.transform.localScale = from;

        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(from, to, t / scaleDuration);
            yield return null;
        }

        obj.transform.localScale = to;
    }
}
