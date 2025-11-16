using UnityEngine;
using System.Collections.Generic;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private float spawnChance = 0.7f;
    [SerializeField] private int minMoney = 1;
    [SerializeField] private int maxMoney = 4;

    private Dictionary<GameObject, System.Action> deathListeners = new Dictionary<GameObject, System.Action>();

    public void RegisterNPC(GameObject npc)
    {
        var npcController = npc.GetComponent<NPCController>();
        if (npcController != null)
        {
            System.Action deathHandler = null;
            deathHandler = () =>
            {
                OnNPCDied(npc.transform.position);
                npcController.OnDeath -= deathHandler;
                deathListeners.Remove(npc);
            };

            npcController.OnDeath += deathHandler;
            deathListeners[npc] = deathHandler;
        }
    }

    private void OnNPCDied(Vector3 deathPosition)
    {
        if (Random.value <= spawnChance)
        {
            SpawnMoney(deathPosition);
        }
    }

    private void SpawnMoney(Vector3 position)
    {
        if (moneyPrefab != null)
        {
            int moneyAmount = Random.Range(minMoney, maxMoney + 1);

            // Создаем экземпляр префаба
            GameObject moneyObject = Instantiate(moneyPrefab, position, Quaternion.identity);

            // Получаем компонент Money и устанавливаем количество
            Money moneyComponent = moneyObject.GetComponent<Money>();
            if (moneyComponent != null)
            {
                moneyComponent.SetAmount(moneyAmount);
            }

            Debug.Log($"Spawned {moneyAmount} money at {position}");
        }
    }

    private void OnDestroy()
    {
        foreach (var listener in deathListeners)
        {
            if (listener.Key != null)
            {
                var npcController = listener.Key.GetComponent<NPCController>();
                if (npcController != null)
                {
                    npcController.OnDeath -= listener.Value;
                }
            }
        }
        deathListeners.Clear();
    }
}