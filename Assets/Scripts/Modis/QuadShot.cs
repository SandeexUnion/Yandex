using UnityEngine;

public class QuadShot : PowerUp
{
    protected override void ApplyPowerUp()
    {
        PowerUpManager powerUpManager = FindAnyObjectByType<PowerUpManager>();
        if (powerUpManager != null)
        {
            powerUpManager.ActivateQuadShot(60f);
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