using UnityEngine;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] private GameObject playerPrefab; // prefab gốc
    [SerializeField] private GameObject player;       // instance đang chơi
  
    [Header("Gameplay References")]
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private ProtectBar protectBar;
    
    [Header("Win Flow Prefabs")]
    [SerializeField] private GameObject pointClearPrefab;
    [SerializeField] private GameObject targetPrefab;
    [Header("UI")]
    [SerializeField] private GameObject progressBarCanvas; // KÉO CANVAS VÀO
    [Header("Spawners")]
    [SerializeField] private RhythmSpawner rhythmSpawner;

    [Header("Spawn Positions")]
    [SerializeField] private Transform pointClearSpawnPos;
    [SerializeField] private Transform targetSpawnPos;
    [Header("Checkpoint")]
    [SerializeField] private Transform defaultPlayerSpawnPos;
    [SerializeField] private MusicTimeline musicTimeline;
    [Header("UI Features")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject resultUI;
    private CheckpointData currentCheckpoint;
    private ResultUI resultUIController;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
      
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseUI == null) return;

            if (Time.timeScale == 1f)
            {
                PauseGame();
                pauseUI.SetActive(true);
            }
            else
            {
                ResumeGame();
                pauseUI.SetActive(false);
            }
        }

    }


    
    public void OnPlayerTakeDamage(float cooldownTime) { 
        if (protectBar != null) { protectBar.StartProcessProtectBar(cooldownTime); } else { Debug.LogWarning("GameManager: ProtectBar reference bị null khi nhận damage!"); } } 
    public void OnMusicFinished()
    {
        Debug.Log("GameManager: Music Finished → Spawn PointClear");

        if (pointClearPrefab != null && pointClearSpawnPos != null)
        {
            Instantiate(pointClearPrefab,
                        pointClearSpawnPos.position,
                        Quaternion.identity);
        }
    }
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }
    public void ExitToMenuGame()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene("JSAB_Menu");
    }

    public void OnPointClearTouched()
    {
        Debug.Log("GameManager: PointClear touched");

        DisableAllGameplayObjects();
        DisableProgressBar();
        DisableSpawner();

        SpawnTarget();
    }
    private void DisableAllGameplayObjects()
    {
        GameObject[] gameplayObjects = GameObject.FindGameObjectsWithTag("GameplayObject");

        foreach (GameObject obj in gameplayObjects)
        {
            obj.SetActive(false);
        }
    }
    public void SaveCheckpoint()
    {
        double time = musicTimeline.director.time;

        currentCheckpoint = new CheckpointData(
            time,
            defaultPlayerSpawnPos.position
        );

        Debug.Log($"✅ Checkpoint saved at time {time}");
    }
    public void OnPlayerDied()
    {
        StartCoroutine(RespawnFromCheckpoint());
    }
    private IEnumerator RespawnFromCheckpoint()
    {
        // ⏸ Pause timeline
        musicTimeline.director.Pause();

        yield return new WaitForSeconds(0.5f);

        // 🧹 Clear toàn bộ bẫy đang tồn tại
        ClearAllGameplayObjects();

        // ⏪ Quay timeline về checkpoint
        if (currentCheckpoint != null)
        {
            musicTimeline.director.time = currentCheckpoint.timelineTime;
        }
        else
        {
            musicTimeline.director.time = 0;
        }
        


        musicTimeline.director.Evaluate();

        // 👤 Spawn lại player
        SpawnPlayerAtCheckpoint();

        yield return null;

        // ▶️ Chạy tiếp timeline
        musicTimeline.director.Play();
    }
    private void ClearAllGameplayObjects()
    {
        var objs = GameObject.FindGameObjectsWithTag("GameplayObject");
        foreach (var o in objs)
        {
            Destroy(o);
        }
    }
    private void SpawnPlayerAtCheckpoint()
    {
        if (player != null)
            Destroy(player);

        Vector3 pos = currentCheckpoint != null
            ? currentCheckpoint.playerSpawnPos
            : defaultPlayerSpawnPos.position;

        player = Instantiate(playerPrefab, pos, Quaternion.identity);

        // 🔗 Bind ProtectBar → Player
        if (protectBar != null)
        {
            protectBar.target = player.transform;

            var movePlayer = player.GetComponent<movePlayer>();
            if (movePlayer != null)
            {
                movePlayer.SetProtectBar(protectBar);
            }
        }
        else
        {
            Debug.LogError("❌ ProtectBar not found when spawning player");
        }
    }




    private void DisableProgressBar()
    {
        if (progressBarCanvas != null)
        {
            progressBarCanvas.SetActive(false);
        }
    }

    private void DisableSpawner()
    {
        if (rhythmSpawner != null)
        {
            rhythmSpawner.enabled = false;
        }
    }

    private void SpawnTarget()
    {
        if (targetPrefab != null && targetSpawnPos != null)
        {
            Instantiate(targetPrefab,
                        targetSpawnPos.position,
                        Quaternion.identity);
        }
    }
    // === FINAL WIN ===
    public void OnTargetReached()
    {
        Debug.Log("🎉 FINAL WIN!");

        // 1. Pause game
        Time.timeScale = 0f;

        // 2. Lấy Player
        if (player == null)
        {
            Debug.LogError("❌ Player not found when win");
            return;
        }

        movePlayer playerScript = player.GetComponent<movePlayer>();
        if (playerScript == null)
        {
            Debug.LogError("❌ movePlayer not found");
            return;
        }

        // 3. Đếm số piece còn lại
        int remainingPieces = 0;
        foreach (var p in playerScript.pieces)
        {
            if (p.activeSelf)
                remainingPieces++;
        }

        // Rank
        string rank = remainingPieces switch
        {
            4 => "S",
            3 => "A",
            2 => "B",
            1 => "C",
            _ => "D"
        };

        // Point reward = int
        int pointReward = remainingPieces;

        // Song name
        string songName = GameSessionData.SelectedSongName;

        // Show result
        if (resultUIController != null)
        {
            resultUIController.ShowResult(songName, rank, pointReward);
        }



    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "JSAB_Level1")
        {
            BindSceneReferences();
            InitLevel();
        }
    }
    void BindSceneReferences()
    {
        var canvas = FindObjectOfType<Canvas>();
        resultUIController = FindObjectOfType<ResultUI>();

        pauseUI = canvas.transform.Find("PauseUI")?.gameObject;
        resultUI = canvas.transform.Find("ResultUI")?.gameObject;

        if (!pauseUI) Debug.LogError("❌ PauseUI not found");
        if (!resultUI) Debug.LogError("❌ ResultUI not found");

        musicTimeline = FindObjectOfType<MusicTimeline>();
        rhythmSpawner = FindObjectOfType<RhythmSpawner>();
        protectBar = FindObjectOfType<ProtectBar>();

        defaultPlayerSpawnPos = GameObject.Find("DefaultPlayerSpawn")?.transform;
        pointClearSpawnPos = GameObject.Find("PointClearSpawnPos")?.transform;
        targetSpawnPos = GameObject.Find("TargetSpawnPos")?.transform;
    }


    void InitLevel()
    {
        Time.timeScale = 1f;
        currentCheckpoint = null;

        // UI
        if (pauseUI != null) pauseUI.SetActive(false);
        if (resultUI != null) resultUI.SetActive(false);

        // Player
        if (player != null)
            Destroy(player);

        SpawnPlayerAtCheckpoint();

        // Offset
       
    }
    private void CleanupBeforeSceneChange()
    {
        // 1. Resume time để Unity cleanup đúng
        Time.timeScale = 1f;

        // 2. Stop timeline
        if (musicTimeline != null && musicTimeline.director != null)
        {
            musicTimeline.director.Stop();
        }

        // 3. Disable spawner
        if (rhythmSpawner != null)
        {
            rhythmSpawner.enabled = false;
        }

        // 4. Stop all coroutines
        StopAllCoroutines();

        // 5. Destroy gameplay objects
        var gameplayObjects = GameObject.FindGameObjectsWithTag("GameplayObject");
        foreach (var obj in gameplayObjects)
        {
            Destroy(obj);
        }

        // 6. Destroy player
        if (player != null)
        {
            Destroy(player);
            player = null;
        }
    }


}
