using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InventoryItem : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private GlowEffect glowEffect;
    private bool wasSelectedLastFrame;
    private bool wasBackpackPressedLastFrame;
    
    [Header("Glow Settings")]
    [SerializeField] public bool isValidItem = true; 
    [SerializeField] private InputActionProperty PickAction;

    [Header("Inventory Data")]
    [SerializeField] private InventoryData inventoryData;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        glowEffect = GetComponent<GlowEffect>();

        if (PickAction.action != null && !PickAction.action.enabled)
            PickAction.action.Enable();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (glowEffect != null)
        {
            if (isValidItem)
                glowEffect.ShowGlow(GlowEffect.GlowColor.Green); 
            else
                glowEffect.ShowGlow(GlowEffect.GlowColor.Red);  
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

        if (isSelectedNow && !wasSelectedLastFrame)
        {
            OnGrab(null);
        }
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

        var action = PickAction.action;
        if (action == null)
            return;

        if (!action.enabled)
            action.Enable();

        bool isBackpackPressedNow = action.IsPressed();

          if (isBackpackPressedNow && !wasBackpackPressedLastFrame)
        {
            if (isValidItem)
            {
                if (inventoryData != null) 
                    inventoryData.AddToBackpack(this.gameObject);
                else
                    Debug.LogWarning("InventoryData is missing! Please assign it in the Inspector.");
            }
            else
            {
                Debug.Log("Item '" + this.gameObject.name + "' tidak penting dibawa!");
            }
        }

        wasBackpackPressedLastFrame = isBackpackPressedNow;
    }
}


