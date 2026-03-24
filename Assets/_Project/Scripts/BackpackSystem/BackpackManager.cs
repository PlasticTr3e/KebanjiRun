using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackpackManager : MonoBehaviour
{
    public static BackpackManager Instance;
    public List<GameObject> inventory = new List<GameObject>();
    private bool isBackpackItemPicked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddToBackpack(GameObject item)
    {
        if (item.name == "Backpack" || isBackpackItemPicked == true)
        {
            if (item.name == "Backpack")
            {
                isBackpackItemPicked = true;
            }

            inventory.Add(item);
            item.SetActive(false); // hide the item
            Debug.Log("Added " + item.name + " to backpack. Total items: " + inventory.Count);
        }
        else
        {
            Debug.Log("Pick backpack first to store other items");
        }
    }
}
