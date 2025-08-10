using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    protected abstract void ApplyPowerUp();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyPowerUp();
            Destroy(gameObject);
        }
    }
}