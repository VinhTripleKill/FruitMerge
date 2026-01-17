using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text nameSongText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text pointRewardText;
    [SerializeField] private Button returnToMenuButton;

    private void Awake()
    {
        returnToMenuButton.onClick.AddListener(OnReturnToMenu);
    }

    public void ShowResult(string songName, string rank, int point)
    {
        nameSongText.text = songName;
        rankText.text = rank;
        pointRewardText.text = point.ToString();

        gameObject.SetActive(true);
    }

    private void OnReturnToMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ExitToMenuGame();
    }
}
