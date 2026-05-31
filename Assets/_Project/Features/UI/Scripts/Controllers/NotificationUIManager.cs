using UnityEngine;
using TMPro;
using System.Collections;

namespace KebanjiRun.Features.UI
{
    public class NotificationUIManager : MonoBehaviour
    {
        public static NotificationUIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject notificationPanel;   
        [SerializeField] private GameObject warningPanel;    
        [SerializeField] private TextMeshProUGUI notificationTxt; 

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
            if (notificationPanel != null) notificationPanel.SetActive(false);
            if (warningPanel != null) warningPanel.SetActive(false);
        }

        public void ShowNotification(string message)
        {
            if (notificationPanel == null || notificationTxt == null) return;

            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);

            notificationTxt.text = message;
            notificationPanel.SetActive(true);

            _hideCoroutine = StartCoroutine(HideNotificationAfterDelay(defaultDisplayDuration));
        }

        public void HideNotification()
        {
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            if (notificationPanel != null) notificationPanel.SetActive(false);
        }

        private IEnumerator HideNotificationAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (notificationPanel != null) notificationPanel.SetActive(false);
        }
    }
}