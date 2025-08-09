using UnityEngine;

public interface IWeapon
{
    float FireRate { get; }
    void Fire(Vector2 direction, Transform firePoint);
    void FireMultiple(Vector2 direction, Transform firePoint, int bulletCount, float spreadAngle);
    void DisplayMuzzleFlash(Transform firePoint);
}
