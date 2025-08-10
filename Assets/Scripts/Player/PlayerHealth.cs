using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;

    public float damageCooldown = 1f;
    private float lastDamageTime;

    public event System.Action OnDeath;

    private PlayerController playerController;

    private void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (Time.time - lastDamageTime > damageCooldown)
        {
            lastDamageTime = Time.time;
            currentHealth -= damageAmount;

            // ����������� �������� ��������� �����
            playerController.PlayHurtAnimation();

            Debug.Log("Player Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // ����������� �������� ������
        playerController.PlayDeathAnimation();

        // ��������� ����������
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (OnDeath != null)
        {
            OnDeath();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        GetComponent<PlayerController>().enabled = true;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}