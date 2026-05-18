using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using KebanjiRun.Features.Interactables;

public class PlayerSpeedByFlashlight : MonoBehaviour
{
    [SerializeField] private DynamicMoveProvider moveProvider;
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float slowSpeed = 2f;

    private TurnFlashLightOnOff flashlight;

    private void Awake()
    {
        if (moveProvider == null)
        {
            moveProvider = GetComponent<DynamicMoveProvider>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryBindFlashlight();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindFlashlight();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBindFlashlight();
    }

    private void TryBindFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.OnFlashlightStateChanged -= HandleFlashlightStateChanged;
            flashlight = null;
        }

        flashlight = FindAnyObjectByType<TurnFlashLightOnOff>();

        if (flashlight != null)
        {
            flashlight.OnFlashlightStateChanged += HandleFlashlightStateChanged;
            HandleFlashlightStateChanged(flashlight.isOn);
        }
    }

    private void UnbindFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.OnFlashlightStateChanged -= HandleFlashlightStateChanged;
            flashlight = null;
        }
    }

    private void HandleFlashlightStateChanged(bool isOn)
    {
        if (moveProvider == null)
        {
            return;
        }

        moveProvider.moveSpeed = isOn ? normalSpeed : slowSpeed;
    }
}