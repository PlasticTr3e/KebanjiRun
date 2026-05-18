using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Environment
{
    public class RisingWater : MonoBehaviour
    {
        [Header("Water Settings")]
        [Tooltip("Berapa meter air naik setiap siklus waktu? (0.05 = 5 cm)")]
        [SerializeField] private float riseAmount = 0.05f;
        
        [Tooltip("Waktu satu siklus dalam detik")]
        [SerializeField] private float timeInterval = 30f;
        
        [Tooltip("Batas maksimum ketinggian air (Sumbu Y)")]
        [SerializeField] private float maxWaterLevel = 1.5f;

        private float _riseRatePerSecond;
        private bool _isRising = false;

        private void Start()
        {
            _riseRatePerSecond = riseAmount / timeInterval;

            GameManager.Instance.OnFloodWarningClosed += StartRising;
            GameManager.Instance.OnGameStateChanged += HandleGameState;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
                GameManager.Instance.OnFloodWarningClosed -= StartRising;
            }
        }

        private void StartRising()
        {
            _isRising = true; 
        }

        private void HandleGameState(GameState state)
        {
            if (state == GameState.PostEvent || state == GameState.GameOver || state == GameState.MissionComplete)
            {
                _isRising = false;
            }
        }

        private void Update()
        {
            if (!_isRising) return;

            if (transform.position.y < maxWaterLevel)
            {
                float newYPosition = transform.position.y + (_riseRatePerSecond * Time.deltaTime);
                newYPosition = Mathf.Min(newYPosition, maxWaterLevel);
                transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
            }
        }
    }
}