using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int amount;

    
    public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddMoney(amount);
            }
            Destroy(gameObject);
        }
    }

}