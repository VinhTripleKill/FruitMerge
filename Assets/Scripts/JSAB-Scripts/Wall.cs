using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
    [Header("Time Config")]
    [SerializeField] private float thoiGianTonTai = 10f;
    [SerializeField] private float thoiGianDiChuyen = 0.5f;

    [Header("Position")]
    [SerializeField] private Transform posA;
    [SerializeField] private Transform posB;

    private void Start()
    {
        transform.position = posA.position;
        StartCoroutine(WallLifeCycle());
    }

    IEnumerator WallLifeCycle()
    {
        yield return Move(posA.position, posB.position, thoiGianDiChuyen);

        float timeWait = thoiGianTonTai - thoiGianDiChuyen * 2f;
        if (timeWait > 0)
            yield return new WaitForSeconds(timeWait);

        yield return Move(posB.position, posA.position, thoiGianDiChuyen);

        Destroy(gameObject);
    }

    IEnumerator Move(Vector3 from, Vector3 to, float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            float t = timer / time;
            transform.position = Vector3.Lerp(from, to, t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
    }
}
