using UnityEngine;
using UnityEngine.Playables;

public class MusicTimeline : MonoBehaviour
{
    public PlayableDirector director;
    public AudioSource music; // chỉ để lấy length

    public float Length => (float)director.duration;
    public float Time => (float)director.time;
    public float NormalizedTime => Length > 0 ? Time / Length : 0f;

    public bool IsFinished => director.state != PlayState.Playing
                              && Time >= Length;

    void Start()
    {
        if (director == null)
        {
            Debug.LogError("MusicTimeline: Missing PlayableDirector");
            return;
        }

        director.Play();
    }
}
