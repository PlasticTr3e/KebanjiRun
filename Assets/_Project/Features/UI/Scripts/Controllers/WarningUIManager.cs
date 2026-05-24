using UnityEngine;
using TMPro;
using System.Collections;

namespace KebanjiRun.Features.UI
{
    public class WarningUIManager : MonoBehaviour
    {
        public static WarningUIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject warningPanel; 
        [SerializeField] private TextMeshProUGUI informationTxt; 

        [Header("Settings")]
        [SerializeField] private float defaultDisplayDuration = 3.5f; 

        private Coroutine _hideCoroutine;

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
            if (warningPanel != null) warningPanel.SetActive(false);
        }

        public void ShowWarning(string message, bool autoHide = true)
        {
            if (warningPanel == null || informationTxt == null) return;
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            
            informationTxt.text = message;
            warningPanel.SetActive(true);

            if (autoHide)
            {
                _hideCoroutine = StartCoroutine(HideWarningAfterDelay(defaultDisplayDuration));
            }
        }

        public void HideWarning()
        {
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            if (warningPanel != null) warningPanel.SetActive(false);
        }

        private IEnumerator HideWarningAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (warningPanel != null) warningPanel.SetActive(false);
        }
    }
}