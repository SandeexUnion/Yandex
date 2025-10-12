using UnityEngine;

public abstract class Weapon : MonoBehaviour, IWeapon
{
    [Header("Weapon Settings")]
    public float fireRate = 0.5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 3f;
    public GameObject muzzleFlashPrefab;

    public float FireRate => fireRate;

    protected float nextFireTime;

    public abstract void Fire(Vector2 direction, Transform firePoint);

    public virtual void FireMultiple(Vector2 direction, Transform firePoint, int bulletCount, float spreadAngle)
    {
        // Реализация по умолчанию - просто вызов Fire несколько раз с небольшим угловым смещением
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float spread = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            float finalAngle = angle + spread;

            Vector2 bulletDirection = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            CreateProjectile(bulletDirection, firePoint.position);
        }
    }

    protected virtual GameObject CreateProjectile(Vector2 direction, Vector3 position)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(projectile, projectileLifetime);

        // Игнорируем коллайдер игрока
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Collider2D playerCollider = transform.parent?.GetComponent<Collider2D>();
        if (projectileCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, playerCollider);
        }

        return projectile;
    }

    public virtual void DisplayMuzzleFlash(Transform firePoint)
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 0.1f);
        }
    }
}