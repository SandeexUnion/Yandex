using UnityEngine;

public class ShotgunWeapon : Weapon
{
    [Header("Shotgun Settings")]
    public int bulletCount = 5;
    public float spreadAngle = 20f;

    public override void Fire(Vector2 direction, Transform firePoint)
    {
        if (Time.time >= nextFireTime)
        {
            FireMultiple(direction, firePoint, bulletCount, spreadAngle);
            DisplayMuzzleFlash(firePoint);
            nextFireTime = Time.time + fireRate;
        }
    }
}
