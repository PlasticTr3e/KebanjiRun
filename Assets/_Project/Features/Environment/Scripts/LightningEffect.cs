using System.Collections;
using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Environment
{
    public class LightningEffect : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private Light stormLight;
        [SerializeField] private float normalIntensity = 0f;
        [SerializeField] private float flashIntensity = 250f;

        [Header("Timing Settings")]
        [SerializeField] private float minTimeBetweenStrikes = 10f;
        [SerializeField] private float maxTimeBetweenStrikes = 30f;

        [Header("Audio Setup")]
        [SerializeField] private AudioSource thunderAudioSource;
        [SerializeField] private AudioClip[] thunderClips;

        private Coroutine _lightningCoroutine;
        private bool _isStormActive = false;

        private void Start()
        {
            if (stormLight != null) stormLight.intensity = normalIntensity;

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
            if (state == GameState.PreEvent || state == GameState.Event)
            {
                if (!_isStormActive)
                {
                    _isStormActive = true;
                    _lightningCoroutine = StartCoroutine(LightningLoop());
                }
            }
            else
            {
                _isStormActive = false;
                if (_lightningCoroutine != null) StopCoroutine(_lightningCoroutine);
            }
        }

        private IEnumerator LightningLoop()
        {
            while (_isStormActive)
            {
                float waitTime = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
                yield return new WaitForSeconds(waitTime);

                yield return StartCoroutine(FlickerLight());

                yield return new WaitForSeconds(Random.Range(0.2f, 1.5f));

                PlayThunderSound();
            }
        }

        private IEnumerator FlickerLight()
        {
            if (stormLight == null) yield break;

            stormLight.intensity = flashIntensity;
            yield return new WaitForSeconds(0.2f);
            stormLight.intensity = normalIntensity;
            yield return new WaitForSeconds(0.1f);

            stormLight.intensity = flashIntensity * 1.2f;
            yield return new WaitForSeconds(0.4f);
            stormLight.intensity = normalIntensity;
        }

        private void PlayThunderSound()
        {
            if (thunderAudioSource != null && thunderClips.Length > 0)
            {
                int randomIndex = Random.Range(0, thunderClips.Length);
                thunderAudioSource.clip = thunderClips[randomIndex];
                thunderAudioSource.Play();
            }
        }
    }
}