using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int highScore = 0;
    private int money = 0;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;
    public event Action<int> OnMoneyChanged;

    private const string HighScoreKey = "HighScore";
    private const string MoneyKey = "Money";
    Observer observer;

    private void Awake()
    {
        observer = GetComponent<Observer>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Загружаем сохраненный рекорд и деньги
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        money = PlayerPrefs.GetInt(MoneyKey, 0);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveMoney();
        OnMoneyChanged?.Invoke(money);
    }

    public void TakeMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
        SaveMoney();
        OnMoneyChanged?.Invoke(money);
    }

    public int CheckMoney()
    {
        return money;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);

        // Проверяем на новый рекорд
        if (currentScore > highScore)
        {
            highScore = currentScore;
            OnHighScoreChanged?.Invoke(highScore);
            SaveHighScore();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;

    // Методы сохранения
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, money);
        PlayerPrefs.Save();
    }

    // Метод для принудительного сохранения всех данных
    public void SaveAllData()
    {
        SaveHighScore();
        SaveMoney();
    }

    // Метод для сброса всех данных (для тестирования)
    public void ResetAllData()
    {
        currentScore = 0;
        money = 0;
        highScore = 0;

        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.DeleteKey(MoneyKey);
        PlayerPrefs.Save();

        OnScoreChanged?.Invoke(currentScore);
        OnMoneyChanged?.Invoke(money);
        OnHighScoreChanged?.Invoke(highScore);
    }
}