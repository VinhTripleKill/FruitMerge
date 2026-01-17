using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SimpleMenu : MonoBehaviour
{
    [Header("Song Info")]
    [SerializeField] private TMP_Text nameSongText;

    public void StartGame()
    {
        // 🔗 Lưu tên bài hát sang Session
        if (nameSongText != null)
        {
            GameSessionData.SelectedSongName = nameSongText.text;
        }
        else
        {
            Debug.LogWarning("⚠ NameSongText chưa được gán, dùng tên mặc định");
            GameSessionData.SelectedSongName = "Unknown Song";
        }

        SceneManager.LoadScene("JSAB_Level1");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
