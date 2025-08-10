using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static int damageMultiplier = 1; // Добавляем множитель урона

    [Header("Settings")]
    public int baseDamage = 1; // Базовый урон
    public string[] targetTags = { "Enemy" };
    public GameObject hitEffect;
    public float lifetime = 3f;

    private int Damage => baseDamage * damageMultiplier; // Фактический урон

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || collision.CompareTag("Player") || collision.CompareTag("Ground")) return;

        foreach (var tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                collision.GetComponent<NPCController>().TakeDamage(Damage); // Используем расчетный урон
            }
        }

        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}