using System.Collections.Generic;
using UnityEngine;

namespace KebanjiRun.Features.Inventory.Data
{
    [CreateAssetMenu(fileName = "NewInventory", menuName = "KebanjiRun/Inventory Data")]
    public class InventoryData : ScriptableObject
    {
        public List<string> collectedItemIDs = new List<string>();
        public bool hasBackpack;

        private void OnEnable()
        {
            collectedItemIDs.Clear();
            hasBackpack = false;
        }

        public bool TryStoreItem(string itemID)
        {
            if (itemID == "Backpack")
            {
                hasBackpack = true;
                if (!collectedItemIDs.Contains(itemID)) collectedItemIDs.Add(itemID);
                return true;
            }

            if (hasBackpack)
            {
                if (!collectedItemIDs.Contains(itemID)) collectedItemIDs.Add(itemID);
                return true;
            }

            return false;
        }
    }
}
