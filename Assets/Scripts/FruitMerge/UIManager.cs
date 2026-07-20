using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main UI")]
    [SerializeField] private GameObject menuUI;

    [SerializeField] private GameObject winUI;

    [SerializeField] private GameObject loseUI;

    [SerializeField] private GameObject pauseUI;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;

    [SerializeField] private Button resumeButton;

    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private TextMeshProUGUI timeCount;

    [Header("Result UI")]
    [SerializeField] private TextMeshProUGUI[] resultScoreTexts;

    [SerializeField] private TextMeshProUGUI[] resultThrowTexts;

    [SerializeField] private TextMeshProUGUI[] resultTimeTexts;

    [Header("Next Fruit")]
    [SerializeField] private Image nextFruitVisual;

    public Image NextFruitVisual => nextFruitVisual;

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

    private void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(
                GameManager.Instance.TogglePause
            );
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(
                GameManager.Instance.ResumeGame
            );
        }
    }

    private void Update()
    {
        UpdateGameplayTime();
    }

    private void UpdateGameplayTime()
    {
        if (timeCount == null)
            return;

        timeCount.text = ScoreManager.Instance .GetFormattedPlayTime();
    }
    // ========= MENU =========

    public void ShowMenu()
    {
        menuUI.SetActive(true);

        winUI.SetActive(false);

        loseUI.SetActive(false);

        pauseUI.SetActive(false);

        pauseButton.gameObject.SetActive(false);
    }

    // ========= GAMEPLAY =========

    public void ShowGameplayUI()
    {
        menuUI.SetActive(false);

        winUI.SetActive(false);

        loseUI.SetActive(false);

        pauseUI.SetActive(false);

        pauseButton.gameObject.SetActive(true);
    }

    // ========= RESULT =========

    public void ShowResultUI(bool isWin)
    {
        UpdateResultUI();

        pauseButton.gameObject.SetActive(false);

        pauseUI.SetActive(false);

        if (isWin)
        {
            winUI.SetActive(true);

            AudioManager.Instance.PlayWinSound();
        }
        else
        {
            loseUI.SetActive(true);

            AudioManager.Instance.PlayLoseSound();
        }
    }

    // ========= SCORE =========

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    // ========= PAUSE =========

    public void UpdatePauseUI(bool paused)
    {
        pauseUI.SetActive(paused);

        pauseButton.gameObject.SetActive(!paused);
    }

    // ========= RESULT DATA =========

    private void UpdateResultUI()
    {
        ScoreManager scoreManager = ScoreManager.Instance;

        foreach (TextMeshProUGUI text in resultScoreTexts)
        {
            if (text != null)
            {
                text.text = scoreManager.Score.ToString();
            }
        }

        foreach (TextMeshProUGUI text in resultThrowTexts)
        {
            if (text != null)
            {
                text.text = scoreManager.ThrowCount.ToString();
            }
        }

        foreach (TextMeshProUGUI text in resultTimeTexts)
        {
            if (text != null)
            {
                text.text = scoreManager.GetFormattedPlayTime();
            }
        }
    }
}