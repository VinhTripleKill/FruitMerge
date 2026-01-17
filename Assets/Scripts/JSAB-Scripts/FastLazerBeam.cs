using UnityEngine;
using System.Collections;

public class FastLazerBeam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer warningLazer;
    [SerializeField] private LineRenderer lazer;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Size Config")]
    [SerializeField] private float length = 20f;
    [SerializeField] private float width = 1f;

    [Header("Time Config")]
    [SerializeField] private float warningTime = 1f;
    [SerializeField] private float lazerTime = 1f;
    [SerializeField] private float lazerAppearTime = 0.1f;
    [SerializeField] private float lazerDisappearTime = 0.1f;

    void Start()
    {
        lazer.gameObject.SetActive(false);
        boxCollider.enabled = false;

        SetupLineBase(warningLazer);
        StartCoroutine(WarningPhase());
    }

    // ================= WARNING =================
    IEnumerator WarningPhase()
    {
        float timer = 0f;

        while (timer < warningTime)
        {
            float t = timer / warningTime;
            float currentWidth = Mathf.Lerp(0f, width, t);

            warningLazer.startWidth = currentWidth;
            warningLazer.endWidth = currentWidth;

            timer += Time.deltaTime;
            yield return null;
        }

        warningLazer.startWidth = width;
        warningLazer.endWidth = width;

        StartLazerPhase();
    }

    // ================= LAZER =================
    void StartLazerPhase()
    {
        warningLazer.gameObject.SetActive(false);

        lazer.gameObject.SetActive(true);
        SetupLineBase(lazer);

        StartCoroutine(LazerAppear());
    }

    IEnumerator LazerAppear()
    {
        float timer = 0f;

        while (timer < lazerAppearTime)
        {
            float t = timer / lazerAppearTime;
            float currentWidth = Mathf.Lerp(0f, width, t);

            lazer.startWidth = currentWidth;
            lazer.endWidth = currentWidth;

            timer += Time.deltaTime;
            yield return null;
        }

        lazer.startWidth = width;
        lazer.endWidth = width;

        SetupCollider();
        yield return new WaitForSeconds(lazerTime);

        StartCoroutine(LazerDisappear());
    }

    IEnumerator LazerDisappear()
    {
        float timer = 0f;

        while (timer < lazerDisappearTime)
        {
            float t = timer / lazerDisappearTime;
            float currentWidth = Mathf.Lerp(width, 0f, t);

            lazer.startWidth = currentWidth;
            lazer.endWidth = currentWidth;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // ================= SETUP =================
    void SetupLineBase(LineRenderer line)
    {
        line.useWorldSpace = false;
        line.positionCount = 2;

        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(length, 0, 0));

        // đưa tâm laser về giữa
        line.transform.localPosition = new Vector3(-length / 2f, 0, 0);
    }

    void SetupCollider()
    {
        boxCollider.enabled = true;
        boxCollider.size = new Vector2(length, width);
        boxCollider.offset = Vector2.zero;
    }
}
