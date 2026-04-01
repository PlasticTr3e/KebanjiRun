using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DoorInteractable : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "Phase2Scene";
    public GameObject promptUI;
    public CanvasGroup fadeCanvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private bool playerInRange = false;
    private bool isTransitioning = false;
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (fadeCanvasGroup != null && isTransitioning)
        {
            Transform cam = Camera.main.transform;
            fadeCanvasGroup.transform.position = cam.position + cam.forward * 0.5f;
            fadeCanvasGroup.transform.rotation = cam.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") || other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand") || other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnInteract);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnInteract);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        playerInRange = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (playerInRange && !isTransitioning)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;
        if (promptUI != null) promptUI.SetActive(false);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            }
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}