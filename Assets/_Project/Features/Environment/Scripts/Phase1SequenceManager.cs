using System.Collections;
using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Environment
{
    public class Phase1SequenceManager : MonoBehaviour
    {
        [Header("Sequence Timing")]
        [SerializeField] private float waitBeforeWarning = 5f;
        [SerializeField] private float fadeDuration = 0.5f;    
        [SerializeField] private float displayDuration = 5f;

        [Header("UI & Effects")]
        [SerializeField] private CanvasGroup phoneNotificationGroup; 
        [SerializeField] private AudioSource notificationSound; 
        [SerializeField] private Light roomLight;

        private void Start()
        {
            if (phoneNotificationGroup != null)
            {
                roomLight.enabled = true;
                phoneNotificationGroup.alpha = 0f;
                phoneNotificationGroup.interactable = false;
                phoneNotificationGroup.blocksRaycasts = false;
            }

            StartCoroutine(SequenceRoutine());
        }

        private IEnumerator SequenceRoutine()
        {
            yield return new WaitForSeconds(waitBeforeWarning);

            if (notificationSound != null) notificationSound.Play();

            if (phoneNotificationGroup != null)
                yield return StartCoroutine(FadeRoutine(phoneNotificationGroup, 1f, fadeDuration));

            roomLight.enabled = false;

            yield return new WaitForSeconds(displayDuration);


            if (phoneNotificationGroup != null)
                yield return StartCoroutine(FadeRoutine(phoneNotificationGroup, 0f, fadeDuration));

            GameManager.Instance.TriggerFloodWarningClosed();
        }

        private IEnumerator FadeRoutine(CanvasGroup cg, float targetAlpha, float duration)
        {
            float startAlpha = cg.alpha;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            cg.alpha = targetAlpha;
        }
    }
}