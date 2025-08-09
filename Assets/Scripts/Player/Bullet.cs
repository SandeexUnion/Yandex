using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 1;
    public string[] targetTags = { "Enemy" };
    public GameObject hitEffect;
    public float lifetime = 3f;

    private void Start()
    {
        // ”ничтожаем пулю через заданное врем€, если она ни во что не попала
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // »гнорируем триггеры и игрока
        if (collision.isTrigger || collision.CompareTag("Player") || collision.CompareTag("Ground")) return;

        // ѕровер€ем цели
        foreach (var tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                collision.GetComponent<NPCController>().TakeDamage();
            }
        }

        // Ёффект попадани€
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}