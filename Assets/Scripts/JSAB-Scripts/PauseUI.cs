using UnityEngine;

public class PauseUIButton : MonoBehaviour
{
    public void ResumeGame()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is NULL");
            return;
        }

        GameManager.Instance.ResumeGame();
    }

    public void ExitToMenu()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is NULL");
            return;
        }

        GameManager.Instance.ExitToMenuGame();
    }
}
