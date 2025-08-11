using UnityEngine;
using Pathfinding;
using System; // Добавляем для Action

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 0.5f;
    public float attackRange = 1.5f; // Дальность атаки
    public int damagePerSecond = 1; // Урон в секунду
    [SerializeField] int HP; private Transform player;
    private AIPath ai;
    private AILerp aiLerp;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth; // Ссылка на PlayerHealth
    private bool isDead = false;
    private float lastAttackTime; // Время последней атаки
    public float attackCooldown = 1f; // Задержка между атаками

    public event Action OnDeath; // Событие смерти

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ai = GetComponent<AIPath>();
        aiLerp = GetComponent<AILerp>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Проверки на null остаются как есть...

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

        if (player != null) // Получаем PlayerHealth при старте
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
        if (player == null) return; // Если игрок уничтожен, ничего не делаем

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // В зоне атаки - останавливаемся и атакуем
            ai.isStopped = true;  // Останавливаем AIPath
            aiLerp.enabled = false; // Отключаем AILerp, чтобы NPC не скользил

            Attack();
        }
        else
        {
            // Вне зоны атаки - преследуем игрока
            ai.isStopped = false; // Возобновляем AIPath
            aiLerp.enabled = true; // Включаем AILerp
            ai.destination = player.position; // Обновляем позицию игрока
        }

        // Анимация движения
        if (animator != null)
        {
            animator.SetFloat("Speed", ai.velocity.magnitude);
        }
    }
    public void TakeDamage(int damage)
    {
        // ... (ваш код получения урона)

        if (HP <= 0 && !isDead) // Проверяем флаг!
        {
            isDead = true; // Устанавливаем флаг
            Die();
        }
        else
        {
            HP--;
        }
    }

    public void Die()
    {
        // ... (ваш код смерти)
        OnDeath?.Invoke(); // Вызываем событие
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
