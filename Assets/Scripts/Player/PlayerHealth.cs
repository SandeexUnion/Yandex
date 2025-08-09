using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    public float damageCooldown = 1f;
    private float lastDamageTime;

    public event System.Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (Time.time - lastDamageTime > damageCooldown)
        {
            lastDamageTime = Time.time;
            currentHealth -= damageAmount;
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
        if (OnDeath != null)
        {
            OnDeath();
        }

        // Не деактивируем игрока сразу, это сделает DeathScreenManager
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}