using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("Weapon Prefabs")]
    public GameObject pistolPrefab;
    public GameObject shotgunPrefab;
    public GameObject machinegunPrefab;

    private PlayerController player;
    private float originalMoveSpeed;
    private int originalDamage = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = FindObjectOfType<PlayerController>();
        originalMoveSpeed = player.moveSpeed;
    }

    // 1. Остановка времени
    public void ActivateTimeStop()
    {
        StartCoroutine(TimeStopCoroutine());
    }

    private IEnumerator TimeStopCoroutine()
    {
        Time.timeScale = 0.2f; // Замедление времени
        yield return new WaitForSecondsRealtime(60); // Реальное время
        Time.timeScale = 1f;
    }

    // 2. Добавление жизни
    public void ActivateExtraLife()
    {
        player.GetComponent<PlayerHealth>().GetHealth();
    }

    // 3. Убийство всех врагов
    public void ActivateKillAllEnemies()
    {
        NPCController[] enemies = FindObjectsOfType<NPCController>();
        foreach (var enemy in enemies)
        {
            enemy.Die();
        }
    }

    // 4. Дробовик
    public void ActivateShotgun()
    {
        player.SetWeapon(shotgunPrefab, 15f);
    }

    // 5. Пулемет
    public void ActivateMachinegun()
    {
        player.SetWeapon(machinegunPrefab, 15f); // Исправлено: 15 секунд вместо 1
    }

    // 6. Стрельба в 4 стороны
    public void ActivateQuadShot(float duration)
    {
        player.ActivateQuadShot(duration); // 15 секунд
    }

    // 7. Увеличение урона
    public void ActivateDamageBoost()
    {
        StartCoroutine(DamageBoostCoroutine());
    }

    private IEnumerator DamageBoostCoroutine()
    {
        Bullet.damageMultiplier = 2; // Статическое поле в классе Bullet
        yield return new WaitForSeconds(60);
        Bullet.damageMultiplier = 1;
    }

    // 8. Увеличение скорости
    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostCoroutine());
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        player.moveSpeed *= 1.5f;
        yield return new WaitForSeconds(60);
        player.moveSpeed = originalMoveSpeed;
    }

    // 9. Увеличение скорострельности
    public void ActivateFireRateBoost()
    {
        StartCoroutine(FireRateBoostCoroutine());
    }

    private IEnumerator FireRateBoostCoroutine()
    {
        // Сохраняем оригинальную скорострельность
        Weapon weapon = player.CurrentWeaponObject.GetComponent<Weapon>();
        float originalFireRate = weapon.fireRate;

        // Увеличиваем скорострельность
        weapon.fireRate /= 2f;

        yield return new WaitForSeconds(60);

        // Восстанавливаем оригинальную скорострельность
        weapon.fireRate = originalFireRate;
    }
}