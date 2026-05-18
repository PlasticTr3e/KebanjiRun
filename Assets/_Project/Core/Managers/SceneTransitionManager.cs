using KebanjiRun.Core.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KebanjiRun.Core.Managers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private float fadeDuration = 1.0f;

        [Header("Scene Names")]
        [SerializeField] private string preEventSceneName = "PreEvent_Scene";
        [SerializeField] private string eventSceneName = "Event_Scene";
        [SerializeField] private string postEventSceneName = "PostEvent_Scene";

        [Header("Player Reference")]
        [SerializeField] private Transform xrRig;

        private string _activeGameplayScene = "";

        private void Start()
        {
            fadeCanvas.alpha = 1;
            fadeCanvas.blocksRaycasts = true;

            _activeGameplayScene = "";

            GameManager.Instance.OnGameStateChanged += HandleStateChanged;

            if (GameManager.Instance.CurrentState == GameState.PreEvent)
            {
                HandleStateChanged(GameState.PreEvent);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState newState)
        {
            string targetScene = GetSceneNameForState(newState);

            if (string.IsNullOrEmpty(targetScene) || targetScene == _activeGameplayScene) return;

            StartCoroutine(TransitionSceneRoutine(_activeGameplayScene, targetScene));
        }

        private string GetSceneNameForState(GameState state)
        {
            return state switch
            {
                GameState.PreEvent => preEventSceneName,
                GameState.Event => eventSceneName,
                GameState.PostEvent => postEventSceneName,
                _ => string.Empty
            };
        }

        private IEnumerator TransitionSceneRoutine(string sceneToUnload, string sceneToLoad)
        {
            yield return Fade(1f);

            if (!string.IsNullOrEmpty(sceneToUnload) && SceneManager.GetSceneByName(sceneToUnload).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }

            yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

            _activeGameplayScene = sceneToLoad;

            PlayerSpawnPoint spawnPoint = FindAnyObjectByType<PlayerSpawnPoint>();
            if (spawnPoint != null && xrRig != null)
            {
                xrRig.position = spawnPoint.transform.position;      
                xrRig.rotation = spawnPoint.transform.rotation;
            }

            yield return Fade(0f);
        }

        private IEnumerator Fade(float targetAlpha)
        {
            fadeCanvas.blocksRaycasts = true;
            float startAlpha = fadeCanvas.alpha;
            float time = 0;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                yield return null;
            }

            fadeCanvas.alpha = targetAlpha;
            fadeCanvas.blocksRaycasts = (targetAlpha > 0);
        }
    }
}