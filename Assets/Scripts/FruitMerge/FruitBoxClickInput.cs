using UnityEngine;
using UnityEngine.EventSystems;

public class FruitBoxClickInput :
    MonoBehaviour,
    IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private ThrowController throwController;

    [SerializeField] private FruitSpawner fruitSpawner;

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        if (GameManager.Instance.GameplayLocked)
            return;

        if (GameManager.Instance.IsPaused)
            return;

        if (GameManager.Instance.IsGameOver)
            return;

        if (GameManager.Instance.IsGameWin)
            return;

        Vector3 screenPos =
            eventData.position;

        Vector3 worldPos =
            mainCamera.ScreenToWorldPoint(
                screenPos
            );

        throwController.TeleportToX(
            worldPos.x
        );

        fruitSpawner.MobileThrow();
    }
}