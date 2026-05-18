using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KebanjiRun.Features.UI
{
    [Serializable]
    public struct ChecklistItemUI
    {
        [Tooltip("Refrence Item Id")]
        public string itemID; 
        
        [Tooltip("Item Text Refrence")]
        public TextMeshProUGUI itemText;
        
        [Tooltip("Checked Indicator Reference")]
        public GameObject checkedIndicator; 
    }

    public class ChecklistUIManager : MonoBehaviour
    {
        public static ChecklistUIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private List<ChecklistItemUI> checklistItems;

        [Header("Settings")]
        [SerializeField] private Color textDimColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        private Color _defaultTextColor;

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
            if (checklistItems.Count > 0 && checklistItems[0].itemText != null)
            {
                _defaultTextColor = checklistItems[0].itemText.color;
            }

            foreach (var item in checklistItems)
            {
                if (item.checkedIndicator != null) 
                    item.checkedIndicator.SetActive(false);
                
                if (item.itemText != null)
                {
                    item.itemText.color = _defaultTextColor;
                    item.itemText.fontStyle = FontStyles.Normal;
                }
            }
        }

        public void MarkItemCollected(string itemID)
        {
            foreach (var item in checklistItems)
            {
                if (item.itemID == itemID)
                {
                    if (item.checkedIndicator != null)
                    {
                        item.checkedIndicator.SetActive(true);
                    }

                    if (item.itemText != null)
                    {
                        item.itemText.color = textDimColor;
                        item.itemText.fontStyle = FontStyles.Strikethrough;
                    }
                    break;
                }
            }
        }
    }
}