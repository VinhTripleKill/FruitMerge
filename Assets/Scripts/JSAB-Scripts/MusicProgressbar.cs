using UnityEngine;
using UnityEngine.UI;

public class MusicProgressBar : MonoBehaviour
{
    public MusicTimeline timeline;
    public Slider slider;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
    }

    void Update()
    {
        if (timeline == null) return;

        slider.value = timeline.NormalizedTime;
    }
}
  