using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalClickEffect : MonoBehaviour
{
    private void Update()
    {
        if (Pointer.current == null)
            return;

        if (!Pointer.current.press.wasPressedThisFrame)
            return;

        Vector2 screenPos = Pointer.current.position.ReadValue();

        AudioManager.Instance?.PlayClickSound();

        ParticalEffect.Instance?.SpawnClickEffect(screenPos);
    }
}