using UnityEngine;
using KebanjiRun.Core.Managers;
using UnityEngine.InputSystem;

namespace KebanjiRun.Features.UI.Controllers
{
    public class MenuUIManager : MonoBehaviour
    {

        [Header("UI Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject missionCompletePanel;

        [Header("Input Pause")]
        [SerializeField] private InputActionProperty pauseAction;

        [Header("Scene Settings")]
        [SerializeField] private string mainMenuSceneName = "MainMenu_Scene";

        private bool _isPaused;
        private void Start()
        {
            HideAllMenus();

            GameManager.Instance.OnGameStateChanged += HandleGameState;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
            }
        }
        private void Update()
        {
            if (pauseAction.action != null && pauseAction.action.WasPressedThisFrame())
            {
                GameState state = GameManager.Instance.CurrentState;
                if (state == GameState.PreEvent || state == GameState.Event || state == GameState.PostEvent)
                {
                    TogglePause();
                }
            }
        }

        private void HandleGameState(GameState state)
        {
            HideAllMenus();

            if (state == GameState.GameOver)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0;
            }else if (state == GameState.MissionComplete)
            {
                missionCompletePanel.SetActive(true);
                Time.timeScale = 0;
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            pausePanel.SetActive(true);

            Time.timeScale = _isPaused ? 0 : 1;
        }

        public void HideAllMenus()
        {
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }


        public void ResumeGame()
        {
            if(_isPaused) TogglePause();
        }

        public void RetryGame()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Core_Scene");
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1;

            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }

    }

}
