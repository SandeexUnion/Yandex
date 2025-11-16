using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buying : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int price;
    [SerializeField] TMP_Text priceText;
    private Observer observer;

    void Start()
    {
        observer = FindAnyObjectByType<Observer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        price = observer.GetPrice();
        priceText.text = price.ToString();
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(FindAnyObjectByType<ScoreManager>().CheckMoney() >= price)
            {
                FindAnyObjectByType<ScoreManager>().TakeMoney(price);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не достаточно денег");
                //Тут надо бахнуть мб вывод диалогового окна продавана что денег нету
            }
        }
    }
}
