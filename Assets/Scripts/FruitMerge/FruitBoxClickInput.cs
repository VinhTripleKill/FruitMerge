using UnityEngine;
using UnityEngine.EventSystems;


public class FruitBoxClickInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private ThrowController throwController;

    [SerializeField] private FruitSpawner fruitSpawner;

    private bool isHolding;
    public void OnPointerDown(PointerEventData eventData)
    {
    if (GameManager.Instance.GameplayLocked) return;
    if (GameManager.Instance.IsPaused) return;
    if (GameManager.Instance.IsGameOver) return;
    if (GameManager.Instance.IsGameWin) return;

    isHolding = true;

    Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);

    throwController.TeleportToX(worldPos.x);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isHolding) return;
    
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
    
        throwController.TeleportToX(worldPos.x);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHolding) return;
    
        isHolding = false;
    
        fruitSpawner.MobileThrow();
    }
}