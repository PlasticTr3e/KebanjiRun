using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private InputActionReference pauseInputAction;

    private bool isPaused = false;

    void Awake()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    void OnEnable()
    {
        if (pauseInputAction != null)
            pauseInputAction.action.performed += OnPauseButtonPressed;
    }

    void OnDisable()
    {
        if (pauseInputAction != null)
            pauseInputAction.action.performed -= OnPauseButtonPressed;
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext ctx)
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
