using UnityEngine;
using TMPro;
using KebanjiRun.Core.Managers;
using UnityEngine.UI;

namespace KebanjiRun.Features.UI
{
    public class GlobalTimer : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject hudCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float totalTimeInSeconds = 360f;

        private float _currentTime;
        private bool _isRunning = false;
        private bool _hasWarnedCriticalTime = false; 

        private void Start()
        {
            _currentTime = totalTimeInSeconds;
            UpdateTimerText();

            if (hudCanvasGroup != null)
            {
                hudCanvasGroup.SetActive(false);
            }

            GameManager.Instance.OnGameStateChanged += HandleGameState;
            GameManager.Instance.OnFloodWarningClosed += StartTimerAndShowHUD;

            HandleGameState(GameManager.Instance.CurrentState);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
                GameManager.Instance.OnFloodWarningClosed -= StartTimerAndShowHUD;
            }
        }

        private void StartTimerAndShowHUD()
        {
            if (hudCanvasGroup != null) hudCanvasGroup.SetActive(true);
            _isRunning = true;

            // KONDISI WAKTU START: Air Mulai Naik (WARNING)
            if (WarningUIManager.Instance != null)
            {
                WarningUIManager.Instance.ShowWarning("Air mulai naik! Segera kumpulkan barang-barang penting Anda!", true);
            }
        }

        private void HandleGameState(GameState state)
        {
            if (state == GameState.PostEvent || state == GameState.GameOver || state == GameState.MissionComplete)
            {
                _isRunning = false;
                if (hudCanvasGroup != null) hudCanvasGroup.SetActive(false); 
            }
        }

        private void Update()
        {
            if (!_isRunning) return;

            _currentTime -= Time.deltaTime;

            // KONDISI WAKTU KRITIS: Sisa waktu di bawah 5 menit / 300 detik (WARNING)
            if (_currentTime <= 300f && !_hasWarnedCriticalTime)
            {
                _hasWarnedCriticalTime = true;
                if (WarningUIManager.Instance != null)
                {
                    WarningUIManager.Instance.ShowWarning("Waktu hampir habis dan air semakin tinggi! Cepat selesaikan daftar cek Anda!", true);
                }
            }

            if (_currentTime <= 0)
            {
                _currentTime = 0;
                _isRunning = false;
                GameManager.Instance.ChangeState(GameState.GameOver);
            }

            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            int minutes = Mathf.FloorToInt(_currentTime / 60);
            int seconds = Mathf.FloorToInt(_currentTime % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (_currentTime <= 60f)
            {
                timerText.color = Color.red;
            }
        }

        public float GetCurrentTime()
        {
            return _currentTime;
        }
    }
}