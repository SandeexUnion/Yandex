using UnityEngine;

public class Division : MonoBehaviour
{
    [SerializeField] GameObject enemyPref; // Make sure this is assigned in the Inspector

    void Start()
    {

    }

    void Update()
    {

    }

    public void SpawnTwoEnemysAfterDeath()
    {
        // Use enemyPref instead of gameObject to avoid infinite recursion
        if (enemyPref != null)
        {
            Instantiate(enemyPref, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
            Instantiate(enemyPref, new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.identity);
        }
        else
        {
            Debug.LogError("enemyPref is not assigned in the Inspector!");
        }
    }
}