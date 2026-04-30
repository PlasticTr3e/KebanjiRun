using UnityEngine;
using TMPro; 

public class HeaderUIManager : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("Durasi countdown dalam detik. Contoh: 840 = 14 menit")]
    [SerializeField] private float totalTime = 840f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeValueText;

    private float timeRemaining;
    private bool timerIsRunning = false;

    private void Start()
    {
        timeRemaining = totalTime;
        RefreshTimerDisplay(timeRemaining);
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
        }
    }

    public void StartTimer() => timerIsRunning = true;
    public void StopTimer() => timerIsRunning = false;

    private void RefreshTimerDisplay(float t)
    {
        if (timeValueText == null) return;
        t = Mathf.Max(0f, t);
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timeValueText.text = string.Format("{0:00}:{1:00}", m, s);
    }
}