using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;


public class DeathScreenManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private Image countdownPie;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text countdownText; // �����������

    [Header("Timer Settings")]
    [SerializeField] private float countdownDuration = 10f;
    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color endColor = Color.red;

    private float currentTime;
    private bool isCountingDown = false;

    private void Start()
    {
        deathScreenPanel.SetActive(false);
        continueButton.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(QuitGame);

        // ��������� ��������� �������
        if (countdownPie != null)
        {
            countdownPie.type = Image.Type.Filled;
            countdownPie.fillMethod = Image.FillMethod.Radial360;
            countdownPie.fillOrigin = (int)Image.Origin360.Top;
            countdownPie.fillClockwise = false;
            countdownPie.fillAmount = 1f;
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnDeath += ShowDeathScreen;
        }
    }

    private void Update()
    {
        if (isCountingDown)
        {
            currentTime -= Time.unscaledDeltaTime;
            UpdateTimerVisual();

            if (currentTime <= 0)
            {
                TimeOut();
            }
        }
    }

    private void UpdateTimerVisual()
    {
        if (countdownPie == null) return;

        // ��������� �������� (�� 1 �� 0)
        float progress = currentTime / countdownDuration;

        // ������������� ���������� (����������� �� ������� �������)
        countdownPie.fillAmount = progress;

        // ������ ���� �� �������� � ��������
        countdownPie.color = Color.Lerp(endColor, startColor, progress);

        // ��������� �������� ������ (�����������)
        if (countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }

    private void ShowDeathScreen()
    {
        deathScreenPanel.SetActive(true);
        currentTime = countdownDuration;
        isCountingDown = true;

        // ������������� �������
        if (countdownPie != null)
        {
            countdownPie.fillAmount = 1f;
            countdownPie.color = startColor;
        }

        Time.timeScale = 0f; // ����� ����
    }

    private void ContinueGame()
    {
        YG2.InterstitialAdvShow();
        isCountingDown = false;
        
        deathScreenPanel.SetActive(false);

        // ��������������� ������
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.GetComponent<PlayerHealth>().ResetHealth();
        }

        Time.timeScale = 1f; // ������������ ����
    }

    private void QuitGame()
    {
        isCountingDown = false;
        Time.timeScale = 1f;
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("Returning to menu...");
    }

    private void TimeOut()
    {
        QuitGame();
    }
}