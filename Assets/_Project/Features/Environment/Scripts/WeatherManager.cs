using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Environment
{
    public class WeatherManager : MonoBehaviour
    {
        [Header("Weather Effects")]
        [SerializeField] private GameObject rainParticleObject;
        [SerializeField] private AudioSource rainAmbientAudio;


        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += HandleGameState;

            HandleGameState(GameManager.Instance.CurrentState);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
        }

        private void HandleGameState(GameState state)
        {
            if (rainParticleObject != null)
            {
                bool isRaining = state == GameState.Event;
                rainParticleObject.SetActive(isRaining);
            }
            if (rainAmbientAudio != null)
            {
                if (state == GameState.PreEvent || state == GameState.Event)
                {
                    if (!rainAmbientAudio.isPlaying) rainAmbientAudio.Play();
                }
                else
                {
                    if (rainAmbientAudio.isPlaying) rainAmbientAudio.Stop();
                }
            }
        }
    }
}