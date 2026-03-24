using UnityEngine;
using System.Collections.Generic;

public class BackpackManager : MonoBehaviour
{
    public static BackpackManager Instance;
    public List<GameObject> inventory = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddToBackpack(GameObject item)
    {
        inventory.Add(item);
        item.SetActive(false); // Hide the item
        Debug.Log("Added " + item.name + " to backpack. Total items: " + inventory.Count);
    }
}
