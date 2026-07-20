using UnityEngine;

[ExecuteAlways]
public class RadialLayout : MonoBehaviour
{
    [SerializeField] private float fDistance = 100f;
    [SerializeField] private float spacingAngle = 30f;
    [SerializeField] private float rotationZ = 0f;

    private void OnEnable()
    {
        CalculateRadial();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CalculateRadial();
    }
#endif

    private void CalculateRadial()
    {
        int childCount = transform.childCount;

        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;

            if (child == null) continue;

            float angle = rotationZ + (spacingAngle * i);

            Vector2 pos = new Vector2( Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * fDistance;

            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

            child.localPosition = pos;
        }
    }
}