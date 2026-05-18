using UnityEngine;
using KebanjiRun.Features.Inventory.Data;

namespace KebanjiRun.Core.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Settings")]
        public int baseScore = 1000;
        public int pointsPerSecond = 2; 
        public int pointsPerExtraItem = 150; 
        [Header("References")]
        [SerializeField] private InventoryData inventoryData;

        public int NPCBonusScore { get; private set; } = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void AddNPCBonus(int amount)
        {
            NPCBonusScore += amount;
            Debug.Log($"Total NPC Bonus: {NPCBonusScore}");
        }

        public int CalculateItemBonus()
        {
            int itemBonus = 0;
            if (inventoryData.collectedItemIDs.Contains("Document")) itemBonus += pointsPerExtraItem;
            if (inventoryData.collectedItemIDs.Contains("Water")) itemBonus += pointsPerExtraItem;
            return itemBonus;
        }
    }
}