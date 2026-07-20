using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreTextPop;

    [Header("Move")]
    [SerializeField] private float moveUpDistance = 0.8f;
    [SerializeField] private float duration = 0.8f;

    [Header("Scale Bounce")]
    [SerializeField] private float bounceScale = 1.35f;

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 baseScale;

    private float timer;

    private float baseFontSize;

    private void Awake()
    {
        startPos = transform.position;
        endPos = startPos + Vector3.up * moveUpDistance;

        baseScale = transform.localScale;

        baseFontSize = scoreTextPop.fontSize;
    }

    public void Setup(int score, int fruitID)
    {
        scoreTextPop.text = $"+{score}";

        // Font Size = 20 + FruitID
        scoreTextPop.fontSize = baseFontSize + fruitID;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);

        // Bay lên
        transform.position = Vector3.Lerp(startPos, endPos, t);

        // Scale Bounce
        float scaleCurve;

        if (t < 0.25f)
        {
            float p = t / 0.25f;

            scaleCurve = Mathf.Lerp(1f, bounceScale, p);
        }
        else
        {
            float p = (t - 0.25f) / 0.75f;

            scaleCurve = Mathf.Lerp(bounceScale, 1f, p);
        }

        transform.localScale = baseScale * scaleCurve;

        // Fade
        Color c = scoreTextPop.color;
        c.a = 1f - t;
        scoreTextPop.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }
}