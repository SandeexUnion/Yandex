using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static int damageMultiplier = 1; // ��������� ��������� �����

    [Header("Settings")]
    public int baseDamage = 1; // ������� ����
    public string[] targetTags = { "Enemy" };
    public GameObject hitEffect;
    public float lifetime = 3f;

    private int Damage => baseDamage * damageMultiplier; // ����������� ����

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
                collision.GetComponent<NPCController>().TakeDamage(Damage); // ���������� ��������� ����
            }
        }

        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}