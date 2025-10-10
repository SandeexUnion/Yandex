using UnityEngine;

public class DamageBoost : PowerUp
{
    protected override void ApplyPowerUp()
    {
        PowerUpManager powerUpManager = FindAnyObjectByType<PowerUpManager>();
        if (powerUpManager != null)
        {
            powerUpManager.ActivateDamageBoost();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyPowerUp();
            Destroy(gameObject);
        }
    }
}