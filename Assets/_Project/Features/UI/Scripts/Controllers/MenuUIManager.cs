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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameState;
                
                HandleGameState(GameManager.Instance.CurrentState);
            }
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
                TogglePause();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                TogglePause();
            }
        }

        public void HandleGameState(GameState state)
        {
            if (state == GameState.GameOver)
            {
                if (pausePanel != null) pausePanel.SetActive(false);
                if (missionCompletePanel != null) missionCompletePanel.SetActive(false);
                
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                
                Time.timeScale = 0; 
                Debug.Log("[MenuUI] Menerima status GAMEOVER dari GlobalTimer. Panel Mission Failed Aktif!");
            }
            else if (state == GameState.MissionComplete)
            {
                if (pausePanel != null) pausePanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                
                if (missionCompletePanel != null) missionCompletePanel.SetActive(true);
                
                Time.timeScale = 0; 
                Debug.Log("[MenuUI] Menerima status MISSION COMPLETE. Panel Mission Success Aktif!");
            }
        }

        public void TogglePause()
        {
            if ((gameOverPanel != null && gameOverPanel.activeSelf) || (missionCompletePanel != null && missionCompletePanel.activeSelf)) 
                return;

            _isPaused = !_isPaused;
            
            if (pausePanel != null) 
                pausePanel.SetActive(_isPaused);

            Time.timeScale = _isPaused ? 0 : 1;
            Debug.Log($"Game Paused: {_isPaused}");
        }

        public void HideAllMenus()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (missionCompletePanel != null) missionCompletePanel.SetActive(false);
        }

        public void ResumeGame()
        {
            if (_isPaused) TogglePause();
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

        public void ExitGame()
        {
            Debug.Log("Keluar dari Game!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}