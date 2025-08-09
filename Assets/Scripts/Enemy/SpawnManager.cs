using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab; // ������ NPC
    public float spawnInterval = 3f; // �������� ����� �������� NPC

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>(); // ������ ����� ������

    private void Start()
    {
        // ��������� �������� ��� ������ NPC
        StartCoroutine(SpawnNPCs());
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            // �������� ��������� ����� ������ �� ������
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // ������� NPC � ��������� �����
            Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

            // ���� �������� ��������
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
