using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Interactables
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
    public class StateTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private GameState targetState;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        }

        private void OnEnable()
        {
            _interactable.selectEntered.AddListener(OnTriggered);
        }

        private void OnDisable()
        {
            _interactable.selectEntered.RemoveListener(OnTriggered);
        }

        private void OnTriggered(SelectEnterEventArgs args)
        {
            GameManager.Instance.ChangeState(targetState);
        }
    }
}