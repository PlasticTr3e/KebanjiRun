using UnityEngine;
using TMPro;
using KebanjiRun.Core.Managers;
using KebanjiRun.Features.Inventory.Data;   

namespace KebanjiRun.Features.UI.Controllers
{
    public class ScoreboardUI : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private TextMeshProUGUI timeInfoText;   
        
        [SerializeField] private TextMeshProUGUI itemInfoText;   
        
        [SerializeField] private TextMeshProUGUI npcInfoText;    
        
        [SerializeField] private TextMeshProUGUI totalScoreText; 

        [Header("Dependencies")]
        [SerializeField] private GlobalTimer globalTimer;
        [SerializeField] private InventoryData inventoryData;

        private void OnEnable()
        {
            CalculateAndDisplayScore();
        }

        private void CalculateAndDisplayScore()
        {
            if (ScoreManager.Instance == null || globalTimer == null || inventoryData == null) return;

            float timeLeft = globalTimer.GetCurrentTime();
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            int timeBonus = Mathf.FloorToInt(timeLeft) * ScoreManager.Instance.pointsPerSecond;
            
            if (timeInfoText) 
            {
                timeInfoText.text = $"{minutes:00}:{seconds:00}";
            }

            int itemCount = inventoryData.collectedItemIDs.Count;
            int baseScore = ScoreManager.Instance.baseScore;
            int itemBonus = ScoreManager.Instance.CalculateItemBonus();
            int totalItemScore = baseScore + itemBonus; 

            if (itemInfoText) 
            {
                itemInfoText.text = $"{itemCount} / 6";
            }

            int npcBonus = ScoreManager.Instance.NPCBonusScore;
            
            if (npcInfoText) 
            {
                npcInfoText.text = npcBonus.ToString();
            }

            int totalScore = timeBonus + totalItemScore + npcBonus;

            if (totalScoreText) 
            {
                totalScoreText.text = totalScore.ToString();
            }
        }
    }
}