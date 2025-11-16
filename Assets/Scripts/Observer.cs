using UnityEngine;

public class Observer : MonoBehaviour
{
    private int price;

    void Start()
    {
        price = 1;
        GeneratePriceByWave();
    }

    public void GeneratePriceByWave()
    {
        // Правильно получаем ссылку на существующий SpawnManager
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();

        if (spawnManager != null)
        {
            // Используем currentWaveIndex из твоего SpawnManager
            int currentWave = spawnManager.CurrentWaveIndex;
            price = Mathf.RoundToInt(currentWave * 1.3f);

            Debug.Log($"Wave {currentWave}, generated price: {price}");
        }
        else
        {
            Debug.LogError("SpawnManager not found in scene!");
        }
    }

    // Метод для получения цены
    public int GetPrice()
    {
        return price;
    }

    // Метод для обновления цены при новой волне
    public void UpdatePriceForNewWave()
    {
        GeneratePriceByWave();
    }
}
