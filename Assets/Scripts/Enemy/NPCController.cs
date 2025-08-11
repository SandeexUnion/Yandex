using UnityEngine;
using Pathfinding;
using System; // ��������� ��� Action

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 0.5f;
    public float attackRange = 1.5f; // ��������� �����
    public int damagePerSecond = 1; // ���� � �������
    [SerializeField] int HP; private Transform player;
    private AIPath ai;
    private AILerp aiLerp;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth; // ������ �� PlayerHealth
    private bool isDead = false;
    private float lastAttackTime; // ����� ��������� �����
    public float attackCooldown = 1f; // �������� ����� �������

    public event Action OnDeath; // ������� ������

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ai = GetComponent<AIPath>();
        aiLerp = GetComponent<AILerp>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // �������� �� null �������� ��� ����...

        ai.maxSpeed = moveSpeed;
        ai.enableRotation = false;
        ai.endReachedDistance = stoppingDistance;

        if (aiLerp == null)
        {
            Debug.LogError("AILerp component not found on NPC.");
            enabled = false;
            return;
        }

        aiLerp.enableRotation = false;

        if (player != null) // �������� PlayerHealth ��� ������
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found on Player.");
                enabled = false;
            }
        }
    }

    private void Update()
    {
        if (player == null) return; // ���� ����� ���������, ������ �� ������

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // � ���� ����� - ��������������� � �������
            ai.isStopped = true;  // ������������� AIPath
            aiLerp.enabled = false; // ��������� AILerp, ����� NPC �� ��������

            Attack();
        }
        else
        {
            // ��� ���� ����� - ���������� ������
            ai.isStopped = false; // ������������ AIPath
            aiLerp.enabled = true; // �������� AILerp
            ai.destination = player.position; // ��������� ������� ������
        }

        // �������� ��������
        if (animator != null)
        {
            animator.SetFloat("Speed", ai.velocity.magnitude);
        }
    }
    public void TakeDamage(int damage)
    {
        // ... (��� ��� ��������� �����)

        if (HP <= 0 && !isDead) // ��������� ����!
        {
            isDead = true; // ������������� ����
            Die();
        }
        else
        {
            HP--;
        }
    }

    public void Die()
    {
        // ... (��� ��� ������)
        OnDeath?.Invoke(); // �������� �������
        Destroy(gameObject);
    }



    private void Attack()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond);
            }
        }
    }

}
