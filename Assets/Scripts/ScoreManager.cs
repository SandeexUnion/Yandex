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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Загружаем только постоянные данные
            highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            // money загружаем из PlayerPrefs только для отображения в магазине и т.д.
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Вызывается при загрузке главного меню
    public void ResetSessionData()
    {
        currentScore = 0;
        money = 0;
        OnScoreChanged?.Invoke(currentScore);
        OnMoneyChanged?.Invoke(money);
    }

    // Вызывается при загрузке игрового уровня
    public void LoadLevelData()
    {
        // Можно загрузить какие-то начальные значения для уровня
        // или оставить текущие значения, если они уже установлены
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    public void TakeMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
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

    // Сохранение денег в PlayerPrefs (вызывается при завершении уровня)
    public void SaveMoneyToPlayerPrefs()
    {
        int totalMoney = PlayerPrefs.GetInt(MoneyKey, 0);
        totalMoney += money; // Добавляем текущие деньги к общим
        PlayerPrefs.SetInt(MoneyKey, totalMoney);
        PlayerPrefs.Save();

        // Сбрасываем текущие деньги после сохранения
        money = 0;
        OnMoneyChanged?.Invoke(money);
    }

    // Получение общих денег из PlayerPrefs (для магазина)
    public int GetTotalMoney()
    {
        return PlayerPrefs.GetInt(MoneyKey, 0);
    }

    // Трата денег из PlayerPrefs (в магазине)
    public void SpendMoney(int amount)
    {
        int totalMoney = PlayerPrefs.GetInt(MoneyKey, 0);
        totalMoney = Mathf.Max(0, totalMoney - amount);
        PlayerPrefs.SetInt(MoneyKey, totalMoney);
        PlayerPrefs.Save();
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    public void SaveAllData()
    {
        SaveHighScore();
    }

    public void ResetAllData()
    {
        currentScore = 0;
        money = 0;
        highScore = 0;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        OnScoreChanged?.Invoke(currentScore);
        OnMoneyChanged?.Invoke(money);
        OnHighScoreChanged?.Invoke(highScore);
    }
}