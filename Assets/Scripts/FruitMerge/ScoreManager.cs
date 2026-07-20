using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Stats")]
    [SerializeField] private int score;

    [SerializeField] private int throwCount;

    [SerializeField] private float playTime;

    private bool isCountingTime;

    public int Score => score;

    public int ThrowCount => throwCount;

    public float PlayTime => playTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdatePlayTime();
    }

    // ========= SCORE =========

    public void AddScore(int amount)
    {
        score += amount;

        UIManager.Instance.UpdateScore(score);
    }

    // ========= THROW =========

    public void AddThrowCount()
    {
        throwCount++;
    }

    // ========= RESET =========

    public void ResetStats()
    {
        score = 0;
        throwCount = 0;
        playTime = 0f;

        UIManager.Instance.UpdateScore(0);
    }

    // ========= TIME =========

    private void UpdatePlayTime()
    {
        if (!isCountingTime) return;

        playTime += Time.deltaTime;
    }

    public void StartPlayTime()
    {
        isCountingTime = true;
    }

    public void StopPlayTime()
    {
        isCountingTime = false;
    }

    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600);

        int minutes = Mathf.FloorToInt((playTime % 3600) / 60);

        int seconds = Mathf.FloorToInt(playTime % 60);

        return string.Format( "{0:00}:{1:00}:{2:00}", hours, minutes, seconds
        );
    }
}