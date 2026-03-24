using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BackpackItem : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private GlowEffect glowEffect;
    private bool wasSelectedLastFrame;
    private bool wasBackpackPressedLastFrame;
    

    [Header("Glow Settings")]
    [SerializeField] public string itemName = "Item";
    [SerializeField] public bool isValidItem = true; // item benar (Hijau), kosongkan jika salah (Merah)
    [SerializeField] private InputActionProperty backpackAction;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        glowEffect = GetComponent<GlowEffect>();

        if (backpackAction.action != null && !backpackAction.action.enabled)
            backpackAction.action.Enable();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (glowEffect != null)
        {
            if (isValidItem)
                glowEffect.ShowGlow(GlowEffect.GlowColor.Green); // Panggil warna Hijau
            else
                glowEffect.ShowGlow(GlowEffect.GlowColor.Red);   // Panggil warna Merah
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (glowEffect != null)
        {
            glowEffect.StopGlow();
        }
    }

    private void Update()
    {
        if (grabInteractable == null)
            return;

        bool isSelectedNow = grabInteractable.isSelected;

        // call OnGrab when selection starts
        if (isSelectedNow && !wasSelectedLastFrame)
        {
            OnGrab(null);
        }
        // call OnRelease when selection ends
        else if (!isSelectedNow && wasSelectedLastFrame)
        {
            OnRelease(null);
        }

        wasSelectedLastFrame = isSelectedNow;

        if (!isSelectedNow)
        {
            wasBackpackPressedLastFrame = false;
            return;
        }

        var action = backpackAction.action;
        if (action == null)
            return;

        if (!action.enabled)
            action.Enable();

        bool isBackpackPressedNow = action.IsPressed();

        // trigger once when button transitions from not-pressed -> pressed
        if (isBackpackPressedNow && !wasBackpackPressedLastFrame)
        {
            if (isValidItem)
            {
                if (BackpackManager.Instance != null)
                    BackpackManager.Instance.AddToBackpack(this.gameObject);
                else
                    Debug.LogWarning("BackpackManager.Instance is null. Make sure BackpackManager exists in the scene.");
            }
            else
            {
                Debug.Log("Item '" + itemName + "' tidak penting dibawa!");
            }
        }

        wasBackpackPressedLastFrame = isBackpackPressedNow;
    }
}


