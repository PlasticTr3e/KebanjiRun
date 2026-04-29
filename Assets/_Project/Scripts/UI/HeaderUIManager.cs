using UnityEngine;
using TMPro; 

public class HeaderUIManager : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("Durasi countdown dalam detik. Contoh: 840 = 14 menit")]
    [SerializeField] private float totalTime = 840f;

    [Header("Score")]
    [SerializeField] private int currentScore = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeValueText;
    [SerializeField] private TextMeshProUGUI scoreValueText;

    private float timeRemaining;
    private bool timerIsRunning = false;

    private void Start()
    {
        timeRemaining = totalTime;
        RefreshTimerDisplay(timeRemaining);
        RefreshScoreDisplay();
        StartTimer();
    }

    private void Update()
    {
        if (!timerIsRunning) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            RefreshTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0f;
            timerIsRunning = false;
            RefreshTimerDisplay(0f);
            OnTimerEnd();
        }
    }

    public void StartTimer() => timerIsRunning = true;
    public void StopTimer() => timerIsRunning = false;

    public void AddScore(int points)
    {
        currentScore += points;
        RefreshScoreDisplay();
    }

    private void RefreshTimerDisplay(float t)
    {
        if (timeValueText == null) return;
        t = Mathf.Max(0f, t);
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timeValueText.text = string.Format("{0:00}:{1:00}", m, s);
    }

    private void RefreshScoreDisplay()
    {
        if (scoreValueText == null) return; 
        scoreValueText.text = currentScore.ToString("D4");
    }

    private void OnTimerEnd()
    {
        Debug.Log("[KebanjiRun] Waktu habis! Skor: " + currentScore);
    }
}