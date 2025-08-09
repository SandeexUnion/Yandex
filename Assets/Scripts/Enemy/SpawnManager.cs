using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab; // Префаб NPC
    public float spawnInterval = 3f; // Интервал между спавнами NPC

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>(); // Список точек спавна

    private void Start()
    {
        // Запускаем корутину для спавна NPC
        StartCoroutine(SpawnNPCs());
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            // Выбираем случайную точку спавна из списка
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // Создаем NPC в выбранной точке
            Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

            // Ждем заданный интервал
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
