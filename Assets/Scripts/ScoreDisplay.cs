using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;

    private void Start()
    {
        // Подписываемся на события
        ScoreManager.Instance.OnScoreChanged += UpdateCurrentScore;
        ScoreManager.Instance.OnHighScoreChanged += UpdateHighScore;

        // Инициализируем начальные значения
        UpdateCurrentScore(ScoreManager.Instance.GetCurrentScore());
        UpdateHighScore(ScoreManager.Instance.GetHighScore());
    }

    private void UpdateCurrentScore(int score)
    {
        currentScoreText.text = $"Score: {score}";
    }

    private void UpdateHighScore(int score)
    {
        highScoreText.text = $"High Score: {score}";
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateCurrentScore;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScore;
        }
    }
}