using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text moneyText;

    private void Start()
    {
        // Подписываемся на события
        ScoreManager.Instance.OnScoreChanged += UpdateCurrentScore;
        ScoreManager.Instance.OnHighScoreChanged += UpdateHighScore;
        ScoreManager.Instance.OnMoneyChanged += UpdateMoney; // Добавлена эта строка

        // Инициализируем начальные значения
        UpdateCurrentScore(ScoreManager.Instance.GetCurrentScore());
        UpdateHighScore(ScoreManager.Instance.GetHighScore());
        UpdateMoney(ScoreManager.Instance.CheckMoney()); // И эта строка
    }

    private void UpdateCurrentScore(int score)
    {
        currentScoreText.text = $"Очки: {score}";
    }

    private void UpdateHighScore(int score)
    {
        highScoreText.text = $"Рекорд: {score}";
    }

    private void UpdateMoney(int money)
    {
        moneyText.text = money.ToString();
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateCurrentScore;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScore;
            ScoreManager.Instance.OnMoneyChanged -= UpdateMoney; // И эта строка
        }
    }
}