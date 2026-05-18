using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.PlayerVR
{
    public class WaterFootstepSFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerCamera; 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] splashClips;

        [Header("Settings")]
        [SerializeField] private float stepDistance = 0.6f; 

        private Vector3 _lastPosition;
        private bool _isInWaterPhase = false;

        private void Start()
        {
            if (playerCamera != null)
                _lastPosition = GetFlatPosition(playerCamera.position);

            GameManager.Instance.OnGameStateChanged += HandleGameState;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
        }

        private void HandleGameState(GameState state)
        {
            _isInWaterPhase = state == GameState.Event;
        }

        private void Update()
        {
            if (!_isInWaterPhase || playerCamera == null || audioSource == null) return;

            Vector3 currentFlatPos = GetFlatPosition(playerCamera.position);
            
            if (Vector3.Distance(_lastPosition, currentFlatPos) >= stepDistance)
            {
                PlaySplashSound();
                _lastPosition = currentFlatPos; 
            }
        }
        private Vector3 GetFlatPosition(Vector3 originalPos)
        {
            return new Vector3(originalPos.x, 0, originalPos.z);
        }

        private void PlaySplashSound()
        {
            if (splashClips.Length > 0 && !audioSource.isPlaying)
            {
                int randomIndex = Random.Range(0, splashClips.Length);
                audioSource.clip = splashClips[randomIndex];
                audioSource.pitch = Random.Range(0.9f, 1.1f); 
                audioSource.Play();
            }
        }
    }
}