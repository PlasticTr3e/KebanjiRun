using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory/Inventory Data")]
public class InventoryData : ScriptableObject
{
    public List<GameObject> inventory = new List<GameObject>();
    public bool isBackpackPicked;
    private void OnEnable()
    {
        inventory.Clear();
        isBackpackPicked = false;
    }

    public void AddToBackpack(GameObject item)
    {
        if (item.name == "Backpack" || isBackpackPicked)
        {
            if (item.name == "Backpack")
            {
                isBackpackPicked = true;
            }

            inventory.Add(item);
            item.SetActive(false); 
            Debug.Log("Added " + item.name + " to backpack. Total items: " + inventory.Count);
        }
        else
        {
            Debug.Log(item.name);
            Debug.Log("Pick Backpack first to store other items");
        }
    }
}