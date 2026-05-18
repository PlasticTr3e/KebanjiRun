using UnityEngine;
using UnityEngine.SceneManagement;

namespace KebanjiRun.Features.UI.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string coreSceneName = "Core_Scene";

        [Header("UI Panels")]
        [Tooltip("Masukkan objek 'Menu' (panel utama) ke sini")]
        [SerializeField] private GameObject mainMenuPanel;
        
        [SerializeField] private GameObject howToPlayPanel;

        private void Start()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        }

        public void StartGame()
        {
            Debug.Log("Memulai Game! Pindah ke: " + coreSceneName);
            SceneManager.LoadScene(coreSceneName);
        }

        public void OpenHowToPlay()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
        }


        public void CloseHowToPlay()
        {
            if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
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