using UnityEngine;
using System.Collections;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.UI.Controllers
{
    public class HUDUIManager : MonoBehaviour
    {
        public static HUDUIManager Instance { get; private set; }

        [Header("HUD Canvas Group / Panel References")]
        [SerializeField] private GameObject hudMainObject; 

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameState;
                StartCoroutine(InitHUDStateDeferred());
            }
            else
            {
                SetHUDVisibility(false);
            }
        }

        private IEnumerator InitHUDStateDeferred()
        {
            yield return new WaitForEndOfFrame();
            if (GameManager.Instance != null)
            {
                HandleGameState(GameManager.Instance.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
            }
        }

        private void HandleGameState(GameState state)
        {
            if (state == GameState.PreEvent || state == GameState.Event || state == GameState.PostEvent)
            {
                SetHUDVisibility(true);
            }
            else
            {
                SetHUDVisibility(false);
                
                if (WarningUIManager.Instance != null) WarningUIManager.Instance.HideWarning();
                if (NotificationUIManager.Instance != null) NotificationUIManager.Instance.HideNotification();
            }
        }

        public void SetHUDVisibility(bool isVisible)
        {
            if (hudMainObject != null)
            {
                hudMainObject.SetActive(isVisible);
            }
        }
    }
}