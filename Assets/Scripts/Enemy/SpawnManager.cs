using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

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
    public Tilemap tilemap;
    AstarPath path;

    [Header("Locations")]
    public List<LocationEnemies> locations;
    private int currentLocationIndex = 0;

    [Header("UI References")]
    public Text waveInfoText;
    public Text waveCountdownText;

    [Header("Obstacles")]
    public List<GameObject> obstaclePrefabs; // Префабы препятствий
    public int maxObstacles = 10; // Максимальное количество препятствий
    public string obstacleTag = "Obstacle"; // Тег для поиска препятствий

    private int currentWaveIndex = 0;
    private int enemiesSpawnedInWave = 0;
    private int strongEnemiesToSpawn = 0;
    private int enemiesRemaining = 0;
    private bool isWaveInProgress = false;
    private EnemyWave currentWave;
    private int currentEnemiesOnScene = 0;

    [Header("Spawn Cooldown")]
    public float spawnCooldown = 0.5f;
    private float lastSpawnTime = 0f;

    // Список для хранения созданных препятствий
    private List<GameObject> currentObstacles = new List<GameObject>();

    private void Awake()
    {
        path = FindAnyObjectByType<AstarPath>();
    }

    private void Start()
    {
        GenerateObstaclesOnGrid();
        StartNewWave();
        path.Scan();
    }

    // Метод для получения случайной позиции на Tilemap
    private Vector3 GetRandomTilePosition()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return Vector3.zero;
        }

        // Получаем границы всех занятых тайлов
        BoundsInt bounds = tilemap.cellBounds;

        // Создаем список всех занятых позиций
        List<Vector3Int> occupiedPositions = new List<Vector3Int>();

        // Проходим по всем ячейкам в границах
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cellPosition))
                {
                    occupiedPositions.Add(cellPosition);
                }
            }
        }

        if (occupiedPositions.Count == 0)
        {
            Debug.LogWarning("No occupied tiles found on tilemap!");
            return Vector3.zero;
        }

        // Выбираем случайную позицию
        Vector3Int randomCell = occupiedPositions[Random.Range(0, occupiedPositions.Count)];

        // Конвертируем клеточную позицию в мировые координаты и устанавливаем Z = -1
        Vector3 worldPosition = tilemap.GetCellCenterWorld(randomCell);
        worldPosition.z = 0f;
        return worldPosition;
    }

    // Метод для удаления старых препятствий
    private void ClearOldObstacles()
    {
        // Удаляем все препятствия из текущего списка
        foreach (GameObject obstacle in currentObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        currentObstacles.Clear();

        // Дополнительно ищем и удаляем все объекты с тегом Obstacle (на всякий случай)
        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
        foreach (GameObject obstacle in existingObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
    }

    private void GenerateObstaclesOnGrid()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0 || tilemap == null)
        {
            Debug.LogWarning("Cannot generate obstacles - missing prefabs or tilemap");
            return;
        }

        // Очищаем старые препятствия
        ClearOldObstacles();

        int obstaclesToSpawn = Random.Range(1, maxObstacles + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomTilePosition();

            // Проверяем, чтобы позиция была валидной
            if (spawnPosition != Vector3.zero)
            {
                GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
                GameObject newObstacle = Instantiate(randomObstacle, spawnPosition, Quaternion.identity);

                // Добавляем тег для легкого поиска
                newObstacle.tag = obstacleTag;

                // Сохраняем ссылку на созданное препятствие
                currentObstacles.Add(newObstacle);
            }
        }

        Debug.Log($"Generated {obstaclesToSpawn} obstacles on tilemap");
    }

    // Метод для проверки, свободна ли позиция
    private bool IsPositionFree(Vector3 position, float checkRadius = 1f)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);
        return colliders.Length == 0;
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

        // Генерируем новые препятствия перед началом волны
        GenerateObstaclesOnGrid();
        path.Scan();
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
        lastSpawnTime = 0f;

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
            if (currentEnemiesOnScene < wave.totalEnemies && Time.time >= lastSpawnTime + spawnCooldown)
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
                lastSpawnTime = Time.time;
                yield return new WaitForSeconds(wave.spawnInterval);
            }
            else
            {
                yield return null;
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

    // Для дебага - отображение границ тайлмапа
    private void OnDrawGizmosSelected()
    {
        if (tilemap != null)
        {
            BoundsInt bounds = tilemap.cellBounds;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(tilemap.transform.position + new Vector3(bounds.center.x, bounds.center.y, 0),
                               new Vector3(bounds.size.x, bounds.size.y, 0));
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