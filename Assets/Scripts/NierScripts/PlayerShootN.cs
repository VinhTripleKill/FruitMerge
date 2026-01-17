using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class PlayerShootN : MonoBehaviour
{
    [Header("Fire Control")]
    [SerializeField] private List<GameObject> bullets;
    private PlayerControllerN player;
    [SerializeField] private float fireDelay = 0.2f;
    [Header("Charged Laser (TEST)")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserChargeTime = 1f;
    [SerializeField] private float laserShowTime = 0.05f;
    [SerializeField] private float laserFadeTime = 0.5f;
    [SerializeField] private float laserDistance = 15f;
    [Header("Charged Laser")]
    [SerializeField] private LineRenderer laserWarning;
    [SerializeField] private LineRenderer laserCore;
    [SerializeField] private LineRenderer laserBeam;
    [SerializeField] private float warningChargeTime = 1f;
    [SerializeField] private float coreChargeTime = 1f;

    [SerializeField] private float warningMaxWidth = 1.5f;
    [SerializeField] private float laserDistanceI = 20f;
    [SerializeField] private float beamWidth = 0.2f;
    private bool laserOToggle = false;
    private Coroutine laserOCoroutine;

    private Coroutine laserRoutine;
    private bool isLaserActive = false;
    private bool isChargingLaser = false;
    private bool StatusFire = false;
    private Coroutine fireCoroutine;
    void Start()   
    {
        player = GetComponent<PlayerControllerN>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleLaserO();
        }

        // ===== LASER INPUT =====
        if (Input.GetKeyDown(KeyCode.I) && !isLaserActive)
        {
            laserRoutine = StartCoroutine(ChargedLaserRoutine());
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            StopLaser();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleFire();
        }
    }
    private void ToggleLaserO()
    {
        laserOToggle = !laserOToggle;

        if (laserOToggle)
        {
            laserOCoroutine = StartCoroutine(AutoLaserOLoop());
        }
        else
        {
            if (laserOCoroutine != null)
            {
                StopCoroutine(laserOCoroutine);
                laserOCoroutine = null;
            }
        }
    }
    private IEnumerator AutoLaserOLoop()
    {
        while (true)
        {
            if (!isChargingLaser)
            {
                yield return StartCoroutine(LazerShootO ());
            }

            yield return new WaitForSeconds(fireDelay);
        }
    }

    private IEnumerator LazerShootO()
    {
        isChargingLaser = true;

        float t = 0f;
        while (t < laserChargeTime)
        {
            if (!laserOToggle)   // 🔥 CHECK TOGGLE, KHÔNG CHECK PHÍM
            {
                isChargingLaser = false;
                yield break;
            }

            t += Time.deltaTime;
            yield return null;
        }

        Vector2 origin = player.firePos.position;
        Vector2 dir = player.firePos.up.normalized;
        Vector2 endPoint = origin + dir * laserDistance;

        laserLine.enabled = true;
        laserLine.SetPosition(0, origin);
        laserLine.SetPosition(1, endPoint);

        yield return new WaitForSeconds(laserShowTime);

        float fade = 0f;
        Color startColor = laserLine.startColor;

        while (fade < laserFadeTime)
        {
            if (!laserOToggle) break;

            fade += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fade / laserFadeTime);
            Color c = new Color(startColor.r, startColor.g, startColor.b, alpha);
            laserLine.startColor = c;
            laserLine.endColor = c;
            yield return null;
        }

        laserLine.enabled = false;
        isChargingLaser = false;
    }


    // ===== CHARGED LASER =====
    private void UpdateLaserPositions()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 origin = player.firePos.position;
        Vector2 mousePos2D = (Vector2)mouseWorld;
        Vector2 dir = (mousePos2D - origin).normalized;
        Vector2 end = origin + dir * laserDistanceI;

        laserWarning.SetPosition(0, origin);
        laserWarning.SetPosition(1, end);

        laserCore.SetPosition(0, origin);
        laserCore.SetPosition(1, end);

        laserBeam.SetPosition(0, origin);
        laserBeam.SetPosition(1, end);
    }

    private void StopLaser()
    {
        if (laserRoutine != null)
        {
            StopCoroutine(laserRoutine);
            laserRoutine = null;
        }

        laserWarning.enabled = false;
        laserCore.enabled = false;
        laserBeam.enabled = false;

        isLaserActive = false;
    }

    private IEnumerator ChargedLaserRoutine()
    {
        isLaserActive = true;

        laserWarning.enabled = true;
        laserCore.enabled = true;
        laserBeam.enabled = false;

        laserWarning.startWidth = 0f;
        laserWarning.endWidth = 0f;
        laserCore.startWidth = 0f;
        laserCore.endWidth = 0f;

        // ================= PHASE 1 — WARNING =================
        float t = 0f;
        while (t < warningChargeTime)
        {
            if (!Input.GetKey(KeyCode.I))
            {
                StopLaser();
                yield break;
            }

            UpdateLaserPositions();

            float w = Mathf.Lerp(0f, warningMaxWidth, t / warningChargeTime);
            laserWarning.startWidth = w;
            laserWarning.endWidth = w;

            t += Time.deltaTime;
            yield return null;
        }

        // ================= PHASE 2 — CORE =================
        t = 0f;
        while (t < coreChargeTime)
        {
            if (!Input.GetKey(KeyCode.I))
            {
                StopLaser();
                yield break;
            }

            UpdateLaserPositions();

            float coreWidth = Mathf.Lerp(0f, warningMaxWidth, t / coreChargeTime);
            laserCore.startWidth = coreWidth;
            laserCore.endWidth = coreWidth;

            t += Time.deltaTime;
            yield return null;
        }

        // ================= PHASE 3 — FIRE =================
        laserWarning.enabled = false;
        laserCore.enabled = false;

        laserBeam.enabled = true;
        laserBeam.startWidth = beamWidth;
        laserBeam.endWidth = beamWidth;

        while (Input.GetKey(KeyCode.I))
        {
            UpdateLaserPositions();
            yield return null;
        }

        StopLaser();
    }


    // ===== AUTO FIRE =====
    private void ToggleFire()
    {
        StatusFire = !StatusFire;

        if (StatusFire)
        {
            fireCoroutine = StartCoroutine(SpawnBulletAuto());
        }
        else
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }
    }

    private IEnumerator SpawnBulletAuto()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay);

            int index = Random.Range(0, bullets.Count);

            Instantiate(
                bullets[index],
                player.firePos.position,
                player.firePos.rotation
            );
        }
    }

}
