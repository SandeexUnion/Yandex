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

    // 1. ��������� �������
    public void ActivateTimeStop()
    {
        StartCoroutine(TimeStopCoroutine());
    }

    private IEnumerator TimeStopCoroutine()
    {
        Time.timeScale = 0.2f; // ���������� �������
        yield return new WaitForSecondsRealtime(60); // �������� �����
        Time.timeScale = 1f;
    }

    // 2. ���������� �����
    public void ActivateExtraLife()
    {
        player.GetComponent<PlayerHealth>().GetHealth();
    }

    // 3. �������� ���� ������
    public void ActivateKillAllEnemies()
    {
        NPCController[] enemies = FindObjectsOfType<NPCController>();
        foreach (var enemy in enemies)
        {
            enemy.Die();
        }
    }

    // 4. ��������
    public void ActivateShotgun()
    {
        player.SetWeapon(shotgunPrefab, 15f);
    }

    // 5. �������
    public void ActivateMachinegun()
    {
        player.SetWeapon(machinegunPrefab, 1f);
        //weapon.GetComponent<Weapon>().fireRate = 0.1f;
    }

    // 6. �������� � 4 �������
    public void ActivateQuadShot()
    {
        StartCoroutine(QuadShotCoroutine());
    }

    private IEnumerator QuadShotCoroutine()
    {
        var originalFirePoints = new List<Transform>(player.firePoints);
        player.firePoints.AddRange(new Transform[] {
            Instantiate(player.firePointUp, player.transform),
            Instantiate(player.firePointDown, player.transform),
            Instantiate(player.firePointLeft, player.transform),
            Instantiate(player.firePointRight, player.transform)
        });

        yield return new WaitForSeconds(60);

        // ���������� ������������ ����� ��������
        player.firePoints = originalFirePoints;
    }

    // 7. ���������� �����
    public void ActivateDamageBoost()
    {
        StartCoroutine(DamageBoostCoroutine());
    }

    private IEnumerator DamageBoostCoroutine()
    {
        Bullet.damageMultiplier = 2; // ����������� ���� � ������ Bullet
        yield return new WaitForSeconds(60);
        Bullet.damageMultiplier = 1;
    }

    // 8. ���������� ��������
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
}