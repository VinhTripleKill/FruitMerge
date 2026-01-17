using UnityEngine;
using System.Collections;

public class RotateCenterLazer : MonoBehaviour
{
    [Header("References")]
    public LineRenderer warningLazer;
    public LineRenderer lazer;
    public BoxCollider2D boxCollider;

    [Header("Config")]
    public float warningLength = 2f;
    public float warningWidth = 0.5f;

    public float lazerLength = 20f;

    public float warningRotateAngle = -90f; // clockwise
    public float warningTime = 1f;
    public float lazerTime = 3f;

    float rotateSpeed; // độ / giây

    void Start()
    {
        lazer.gameObject.SetActive(false);
        boxCollider.enabled = false;

        SetupLine(warningLazer, warningLength, warningWidth);

        rotateSpeed = warningRotateAngle / warningTime;
        StartCoroutine(WarningPhase());
    }

    IEnumerator WarningPhase()
    {
        yield return RotateForTime(warningTime);
        StartLazerPhase();
    }

    void StartLazerPhase()
    {
        warningLazer.gameObject.SetActive(false);

        lazer.gameObject.SetActive(true);
        SetupLine(lazer, lazerLength, warningWidth);

        SetupCollider();

        StartCoroutine(LazerPhase());
    }

    IEnumerator LazerPhase()
    {
        yield return RotateForTime(lazerTime);
        Destroy(gameObject);
    }

    IEnumerator RotateForTime(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void SetupLine(LineRenderer line, float length, float width)
    {
        line.useWorldSpace = false;
        line.positionCount = 2;
        line.startWidth = width;
        line.endWidth = width;

        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(length, 0, 0));

        // đưa tâm line về giữa
        line.transform.localPosition = new Vector3(-length / 2f, 0, 0);
    }

    void SetupCollider()
    {
        boxCollider.enabled = true;
        boxCollider.size = new Vector2(lazerLength, warningWidth);
        boxCollider.offset = Vector2.zero;
    }
}
