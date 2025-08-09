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
        // ���������� ���� ����� �������� �����, ���� ��� �� �� ��� �� ������
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������� �������� � ������
        if (collision.isTrigger || collision.CompareTag("Player") || collision.CompareTag("Ground")) return;

        // ��������� ����
        foreach (var tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                collision.GetComponent<NPCController>().TakeDamage();
            }
        }

        // ������ ���������
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}