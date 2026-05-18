using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; 

namespace KebanjiRun.Features.Environment
{
    [RequireComponent(typeof(XRBaseInteractable))]
    public class AutoFindXRManager : MonoBehaviour
    {
        private void Awake()
        {
            var interactable = GetComponent<XRBaseInteractable>();
            var manager = FindAnyObjectByType<XRInteractionManager>();

            if (interactable != null && manager != null)
            {
                interactable.interactionManager = manager;
            }
        }
    }
}