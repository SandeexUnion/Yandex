using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class LocationEnemies
{
    public string locationName;
    public List<GameObject> normalEnemies;
    public List<GameObject> strongEnemies;
    public List<Transform> spawnPoints;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Wave Generation Settings")]
    public int initialEnemiesPerWave = 5;
    public float enemyIncreasePerWave = 2f;
    public float initialStrongEnemyPercent = 10f;
    public float strongEnemyIncreasePerWave = 2f;
    public float spawnInterval = 3f;
    public float delayAfterWave = 5f;

    [Header("Locations")]
    public List<LocationEnemies> locations;
    private int currentLocationIndex = 0;

    [Header("UI References")]
    public Text waveInfoText;
    public Text waveCountdownText;

    private int currentWaveIndex = 0;
    private int enemiesSpawnedInWave = 0;
    private int strongEnemiesToSpawn = 0;
    private int enemiesRemaining = 0;
    private bool isWaveInProgress = false;
    private EnemyWave currentWave;

    private int currentEnemiesOnScene = 0;

    [Header("Spawn Cooldown")]
    public float spawnCooldown = 0.5f; // Кулдаун на спавн
    private float lastSpawnTime = 0f;

    private void Start()
    {
        StartNewWave();
    }

    private IEnumerator StartWaveWithDelay(float delay)
    {
        if (waveCountdownText != null)
        {
            waveCountdownText.gameObject.SetActive(true);
            float timer = delay;
            while (timer > 0)
            {
                waveCountdownText.text = $"Next wave in: {Mathf.Ceil(timer)}";
                timer -= Time.deltaTime;
                yield return null;
            }
            waveCountdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        StartNewWave();
    }

    private void StartNewWave()
    {
        currentWaveIndex++;
        currentWave = GenerateWave(currentWaveIndex);
        enemiesSpawnedInWave = 0;
        strongEnemiesToSpawn = Mathf.RoundToInt(currentWave.totalEnemies * currentWave.strongEnemyPercent / 100f);
        enemiesRemaining = currentWave.totalEnemies;
        currentEnemiesOnScene = 0;
        lastSpawnTime = 0f; // Сбрасываем время последнего спавна

        UpdateWaveUI();
        StartCoroutine(SpawnWave(currentWave));
    }

    private EnemyWave GenerateWave(int waveIndex)
    {
        EnemyWave wave = new EnemyWave();
        wave.waveName = $"Wave {waveIndex}";
        wave.totalEnemies = initialEnemiesPerWave + Mathf.RoundToInt((waveIndex - 1) * enemyIncreasePerWave);
        wave.strongEnemyPercent = initialStrongEnemyPercent + ((waveIndex - 1) * strongEnemyIncreasePerWave);
        wave.spawnInterval = spawnInterval;
        wave.delayAfterWave = delayAfterWave;
        return wave;
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        isWaveInProgress = true;
        var currentLocation = locations[currentLocationIndex];

        while (enemiesSpawnedInWave < wave.totalEnemies)
        {
            if (currentEnemiesOnScene < wave.totalEnemies && Time.time >= lastSpawnTime + spawnCooldown) // Проверяем кулдаун
            {
                Transform spawnPoint = currentLocation.spawnPoints[Random.Range(0, currentLocation.spawnPoints.Count)];

                bool spawnStrongEnemy = strongEnemiesToSpawn > 0 &&
                                         Random.value <= (float)strongEnemiesToSpawn / (wave.totalEnemies - enemiesSpawnedInWave);

                GameObject enemyToSpawn;
                if (spawnStrongEnemy && currentLocation.strongEnemies.Count > 0)
                {
                    enemyToSpawn = currentLocation.strongEnemies[Random.Range(0, currentLocation.strongEnemies.Count)];
                    strongEnemiesToSpawn--;
                }
                else
                {
                    enemyToSpawn = currentLocation.normalEnemies[Random.Range(0, currentLocation.normalEnemies.Count)];
                }

                GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
                NPCController enemyController = spawnedEnemy.GetComponent<NPCController>();
                if (enemyController != null)
                {
                    enemyController.OnDeath += () => EnemyDied();
                }

                enemiesSpawnedInWave++;
                currentEnemiesOnScene++;
                UpdateWaveUI();
                lastSpawnTime = Time.time; // Обновляем время последнего спавна
                yield return new WaitForSeconds(wave.spawnInterval); // Все равно ждем spawnInterval
            }
            else
            {
                yield return null; // Ждем до следующего кадра
            }
        }

        isWaveInProgress = false;
        yield return null;
    }

    private void EnemyDied()
    {
        enemiesRemaining--;
        currentEnemiesOnScene--;
        UpdateWaveUI();

        if (enemiesRemaining <= 0)
        {
            if (waveInfoText != null)
                waveInfoText.text = $"Wave {currentWaveIndex} completed!";

            StartCoroutine(StartWaveWithDelay(currentWave.delayAfterWave));
        }
    }

    private void UpdateWaveUI()
    {
        if (waveInfoText == null) return;

        if (isWaveInProgress)
        {
            waveInfoText.text = $"{currentWave.waveName}\n" +
                               $"Enemies: {enemiesSpawnedInWave}/{currentWave.totalEnemies}\n" +
                               $"Strong left: {strongEnemiesToSpawn}\n" +
                               $"Remaining: {enemiesRemaining}";
        }
        else
        {
            waveInfoText.text = $"Wave {currentWaveIndex} completed!\n" +
                               $"Next wave in progress...";
        }
    }

    public void SetLocation(int locationIndex)
    {
        if (locationIndex >= 0 && locationIndex < locations.Count)
        {
            currentLocationIndex = locationIndex;
            Debug.Log($"Location changed to: {locations[currentLocationIndex].locationName}");
            UpdateWaveUI();
        }
    }
}

[System.Serializable]
public class EnemyWave
{
    public string waveName;
    public int totalEnemies;
    public float strongEnemyPercent;
    public float spawnInterval;
    public float delayAfterWave;
}
