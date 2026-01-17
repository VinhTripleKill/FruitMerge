using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
public class TakeDamageEffect : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color Color = Color.white;
    [SerializeField] private float Duration = 0.25f;
    
    private SpriteRenderer[] _spriteRenderer;
    private Material[] materials;
    private Coroutine flashCoroutine;
    [SerializeField]private AnimationCurve flashCurve;
    private void Awake()
    {
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        materials = new Material[_spriteRenderer.Length];
        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            materials[i] = _spriteRenderer[i].material;
        }
    }   
    public void CallDamageFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashDamage());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator FlashDamage()
    {
        SetColor();
        float elapsed = 0f;
        float currentFlashAmount = 0f;
        while (elapsed<Duration) {
            elapsed += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f,flashCurve.Evaluate(elapsed), elapsed/Duration);
            SetIndex(currentFlashAmount);
            yield return null;
        }
    }
    private void SetColor()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_FlashColor", Color);
            
        }
    }
    private void SetIndex(float FlashDuration)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_FlashAmount", FlashDuration);
        }
    }
}
