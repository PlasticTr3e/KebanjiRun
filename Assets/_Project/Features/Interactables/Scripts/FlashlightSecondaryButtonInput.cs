using UnityEngine;
using UnityEngine.InputSystem;
using KebanjiRun.Features.Interactables;

public class FlashlightSecondaryButtonInput : MonoBehaviour
{
    [SerializeField] private TurnFlashLightOnOff flashlight;
    [SerializeField] private InputActionReference primaryButton;
    [SerializeField] private Transform equippedHand;

    private void Awake()
    {
        if (equippedHand == null)
            equippedHand = transform;
    }

    private void OnEnable()
    {
        if (primaryButton != null)
        {
            primaryButton.action.performed += OnPrimaryButtonPressed;
            primaryButton.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (primaryButton != null)
        {
            primaryButton.action.performed -= OnPrimaryButtonPressed;
            primaryButton.action.Disable();
        }
    }

    private void OnPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (flashlight == null)
        {
            flashlight = FindFlashlightInLoadedScenes();
            if (flashlight == null) return;
        }

        if (equippedHand == null) return;

        if (flashlight.transform.parent == equippedHand || flashlight.transform.IsChildOf(equippedHand))
        {
            flashlight.ToggleLight();
        }
    }

    private TurnFlashLightOnOff FindFlashlightInLoadedScenes()
    {
        var found = FindAnyObjectByType<TurnFlashLightOnOff>();
        if (found != null) return found;

        var all = Resources.FindObjectsOfTypeAll<TurnFlashLightOnOff>();
        return (all != null && all.Length > 0) ? all[0] : null;
    }
}