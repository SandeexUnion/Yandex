using UnityEngine;

public class PistolWeapon : Weapon
{
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        if (Time.time >= nextFireTime)
        {
            CreateProjectile(direction, firePoint.position);
            DisplayMuzzleFlash(firePoint);
            nextFireTime = Time.time + fireRate;
        }
    }
}
