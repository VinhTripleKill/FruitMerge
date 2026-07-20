using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseTrigger : MonoBehaviour
{
    [Header("Lose Settings")]
    [SerializeField] private float loseDelay = 3f;

    // Fruit nào đang nằm trong trigger
    private HashSet<Fruit> fruitsInside = new HashSet<Fruit>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fruit fruit = collision.GetComponent<Fruit>();

        if (fruit == null) return;

        // Thêm vào danh sách
        fruitsInside.Add(fruit);

        // Start countdown riêng cho fruit này
        StartCoroutine( LoseCountdown(fruit));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Fruit fruit = collision.GetComponent<Fruit>();

        if (fruit == null) return;

        // Fruit đã thoát vùng danger
        fruitsInside.Remove(fruit);
    }

    private IEnumerator LoseCountdown(Fruit fruit)
    {
        yield return new WaitForSeconds( loseDelay );

        // Sau delay vẫn còn nằm trong vùng
        if (fruit != null && fruitsInside.Contains(fruit))
        {
            GameManager.Instance.GameOver();
        }
    }
}