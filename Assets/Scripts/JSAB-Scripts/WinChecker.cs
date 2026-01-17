using UnityEngine;
using UnityEngine.Playables;

public class WinChecker : MonoBehaviour
{
    public MusicTimeline timeline;
    private bool hasWon = false;

    void Start()
    {
        if (timeline == null || timeline.director == null)
        {
            Debug.LogError("WinChecker: Missing MusicTimeline or Director");
            return;
        }

        timeline.director.stopped += OnTimelineStopped;
    }

    // WinChecker.cs → sửa thành như này hoặc comment luôn
    private void OnTimelineStopped(PlayableDirector director)
    {
         if (hasWon) return;
         hasWon = true;
         OnWin();  
    }

    void OnWin()
    {
        Debug.Log("WIN - Music finished!");
        GameManager.Instance.OnMusicFinished();
    }

    private void OnDestroy()
    {
        if (timeline != null && timeline.director != null)
        {
            timeline.director.stopped -= OnTimelineStopped;
        }
    }
}
