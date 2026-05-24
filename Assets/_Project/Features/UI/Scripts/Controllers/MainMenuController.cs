using UnityEngine;
using UnityEngine.SceneManagement;

namespace KebanjiRun.Features.UI.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string coreSceneName = "Core_Scene";

        [Header("UI Panels (Canvas)")]
        [Tooltip("Masukkan objek 'Menu' (Canvas utama) ke sini")]
        [SerializeField] private Canvas mainMenuCanvas;
        
        [Tooltip("Masukkan objek 'HowToPlay' (Canvas panduan) ke sini")]
        [SerializeField] private Canvas howToPlayCanvas;

        private void Start()
        {
            if (mainMenuCanvas != null) mainMenuCanvas.enabled = true;
            if (howToPlayCanvas != null) howToPlayCanvas.enabled = false;
        }

        public void StartGame()
        {
            Debug.Log("Memulai Game! Pindah ke: " + coreSceneName);
            SceneManager.LoadScene(coreSceneName);
        }

        public void OpenHowToPlay()
        {
            if (mainMenuCanvas != null) mainMenuCanvas.enabled = false;
            if (howToPlayCanvas != null) howToPlayCanvas.enabled = true;
        }

        public void CloseHowToPlay()
        {
            if (howToPlayCanvas != null) howToPlayCanvas.enabled = false;
            if (mainMenuCanvas != null) mainMenuCanvas.enabled = true;
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