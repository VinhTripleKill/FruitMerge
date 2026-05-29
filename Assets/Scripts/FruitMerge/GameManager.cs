using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    [SerializeField] private bool isGameOver;
    [SerializeField] private bool isGameWin;
    [SerializeField] private bool isPaused;
    [SerializeField] private bool gameplayLocked = true;
    [SerializeField] private float timeWait = 2f;
    private bool waitingEndUI;

    public bool IsGameOver => isGameOver;
    public bool IsGameWin => isGameWin;
    public bool IsPaused => isPaused;
    public bool GameplayLocked => gameplayLocked;

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
        Time.timeScale = 1f;

        UIManager.Instance.ShowMenu();
    }

    // ========= PLAY =========

    public void PlayGame()
    {
        isGameOver = false;
        isGameWin = false;
        isPaused = false;

        gameplayLocked = false;
        waitingEndUI = false;

        Time.timeScale = 1f;

        ScoreManager.Instance.ResetStats();
        ScoreManager.Instance.StartPlayTime();

        UIManager.Instance.ShowGameplayUI();
        ThrowController throwController =
    FindFirstObjectByType<ThrowController>();

        if (throwController != null)
        {
            throwController.ResetPosition();
        }
        FruitSpawner spawner =
            FindFirstObjectByType<FruitSpawner>();

        if (spawner != null)
        {
            spawner.StartGameSpawn();
        }
    }

    // ========= EXIT =========

    public void ExitGame()
    {
        gameplayLocked = true;

        isPaused = false;
        isGameOver = false;
        isGameWin = false;

        waitingEndUI = false;

        Time.timeScale = 1f;

        ScoreManager.Instance.ResetStats();

        UIManager.Instance.ShowMenu();

        FruitSpawner spawner =
            FindFirstObjectByType<FruitSpawner>();

        if (spawner != null)
        {
            spawner.ClearAllFruits();
        }
    }

    // ========= GAME OVER =========

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        ScoreManager.Instance.StopPlayTime();

        StartCoroutine(
            WaitScoreStableThenShowUI(false)
        );
    }

    // ========= GAME WIN =========

    public void GameWin()
    {
        if (isGameWin)
            return;

        isGameWin = true;

        ScoreManager.Instance.StopPlayTime();

        StartCoroutine(
            WaitScoreStableThenShowUI(true)
        );
    }

    // ========= WAIT SCORE =========

    private IEnumerator WaitScoreStableThenShowUI(
        bool isWin
    )
    {
        if (waitingEndUI)
            yield break;

        waitingEndUI = true;

        while (true)
        {
            int oldScore =
                ScoreManager.Instance.Score;

            yield return new WaitForSeconds(timeWait);

            if (oldScore ==
                ScoreManager.Instance.Score)
            {
                break;
            }
        }

        gameplayLocked = true;

        UIManager.Instance.ShowResultUI(isWin);
    }

    // ========= PAUSE =========

    public void TogglePause()
    {
        if (gameplayLocked)
            return;

        isPaused = !isPaused;

        Time.timeScale =
            isPaused ? 0f : 1f;

        if (isPaused)
        {
            ScoreManager.Instance.StopPlayTime();
        }
        else
        {
            ScoreManager.Instance.StartPlayTime();
        }

        UIManager.Instance.UpdatePauseUI(isPaused);
    }

    public void ResumeGame()
    {
        if (gameplayLocked)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        ScoreManager.Instance.StartPlayTime();

        UIManager.Instance.UpdatePauseUI(false);
    }

    // ========= PLAY AGAIN =========

    public void PlayAgain()
    {
        PlayGame();
    }

    // ========= QUIT =========

    public void QuitGame()
    {
        Application.Quit();
    }
}