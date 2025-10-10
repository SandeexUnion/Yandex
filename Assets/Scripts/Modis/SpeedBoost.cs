using UnityEngine;

public class SpeedBost : PowerUp
{
    protected override void ApplyPowerUp()
    {
        PowerUpManager powerUpManager = FindAnyObjectByType<PowerUpManager>();
        if (powerUpManager != null)
        {
            powerUpManager.ActivateSpeedBoost();
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
