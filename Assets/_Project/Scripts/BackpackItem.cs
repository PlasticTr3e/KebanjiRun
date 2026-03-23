using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;
using TMPro;

public class BackpackItem : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private GameObject promptUI;
    private TextMeshProUGUI promptText;
    private GlowEffect glowEffect; 

    [Header("Glow Settings")]
    [SerializeField] public string itemName = "Item";
    [SerializeField] public bool isValidItem = true; // item benar (Hijau), kosongkan jika salah (Merah)

    [SerializeField] private InputActionProperty backpackAction;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        glowEffect = GetComponent<GlowEffect>();
        SetupPromptUI();
    }

    private void SetupPromptUI()
    {
        // Create a basic World Space Canvas for the prompt
        GameObject canvasGo = new GameObject("BackpackPromptCanvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler));
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.transform.SetParent(this.transform);
        canvasGo.transform.localPosition = new Vector3(0, 0.5f, 0); // Above the item
        canvasGo.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        GameObject textGo = new GameObject("PromptText", typeof(TextMeshProUGUI));
        textGo.transform.SetParent(canvasGo.transform, false);
        promptText = textGo.GetComponent<TextMeshProUGUI>();
        promptText.text = "Press [Secondary Button] to put in backpack";
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.fontSize = 40;

        promptUI = canvasGo;
        promptUI.SetActive(false);
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        promptUI.SetActive(true);
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
        promptUI.SetActive(false);
        if (glowEffect != null)
        {
            glowEffect.StopGlow(); 
        }
    }

    private void Update()
    {
        if (grabInteractable.isSelected)
        {
            // Face the camera
            if (Camera.main != null)
                promptUI.transform.LookAt(Camera.main.transform);

            // Check if backpack button is pressed
            if (backpackAction.action != null && backpackAction.action.WasPressedThisFrame())
            {
                if (isValidItem)
                {
                    BackpackManager.Instance.AddToBackpack(this.gameObject);
                }
                else
                {
                    Debug.Log("Item '" + itemName + "' tidak penting dibawa!");
                }
            }
        }
    }
}
