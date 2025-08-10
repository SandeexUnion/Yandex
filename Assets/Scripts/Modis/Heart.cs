using UnityEngine;

public class Heart : MonoBehaviour
{
    private PlayerHealth player;
    private void Awake()
    {
        player = FindAnyObjectByType<PlayerHealth>();
    }

    void Heal()
    {
        player.GetHealth();
    }
}
