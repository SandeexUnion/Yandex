using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class EnemyWave
{
    public string waveName; 
    public int totalEnemies;
    public float strongEnemyPercent;
    public float spawnInterval = 3f;
    public float delayAfterWave = 5f; 
}

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
    [Header("Wave Settings")]
    public List<EnemyWave> waves;
    private int currentWaveIndex = 0;
    private int enemiesSpawnedInWave = 0;
    private int strongEnemiesToSpawn = 0;
    private bool isWaveInProgress = false;

    [Header("Locations")]
    public List<LocationEnemies> locations;
    private int currentLocationIndex = 0;

    [Header("UI References")]
    public Text waveInfoText; // Текст для отображения информации о волне
    public Text waveCountdownText; // Текст для отсчета времени до следующей волны

    private void Start()
    {
        UpdateWaveUI();
        StartCoroutine(StartWaveWithDelay(3f)); // Начинаем первую волну с небольшой задержкой
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
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            if (waveInfoText != null)
                waveInfoText.text = "All waves completed!";
            return;
        }

        enemiesSpawnedInWave = 0;
        var currentWave = waves[currentWaveIndex];
        strongEnemiesToSpawn = Mathf.RoundToInt(currentWave.totalEnemies * currentWave.strongEnemyPercent / 100f);

        UpdateWaveUI();
        StartCoroutine(SpawnWave(currentWave));
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        isWaveInProgress = true;
        var currentLocation = locations[currentLocationIndex];

        while (enemiesSpawnedInWave < wave.totalEnemies)
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

            Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
            enemiesSpawnedInWave++;

            UpdateWaveUI();
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // Волна завершена
        isWaveInProgress = false;
        currentWaveIndex++;

        // Обновляем UI после волны
        if (waveInfoText != null)
            waveInfoText.text = $"Wave {currentWaveIndex} completed!";

        // Задержка перед следующей волной
        yield return StartCoroutine(StartWaveWithDelay(wave.delayAfterWave));
    }

    private void UpdateWaveUI()
    {
        if (waveInfoText == null) return;

        if (isWaveInProgress)
        {
            var currentWave = waves[currentWaveIndex];
            waveInfoText.text = $"{currentWave.waveName}\n" +
                               $"Enemies: {enemiesSpawnedInWave}/{currentWave.totalEnemies}\n" +
                               $"Strong left: {strongEnemiesToSpawn}";
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