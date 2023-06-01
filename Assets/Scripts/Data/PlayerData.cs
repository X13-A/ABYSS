using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData m_Instance;
    public static PlayerData Instance => m_Instance;

    private float maxHealth;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    private float health;

    public float Health { get { return health; } set { health = value; } }

    private Dictionary<ItemId, InventoryItem> inventory = new Dictionary<ItemId, InventoryItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void SaveInventory(Dictionary<ItemId, InventoryItem> inventory)
    {
        // Use clone constructor to loose references
        this.inventory = inventory.ToDictionary(entry => entry.Key, entry => new InventoryItem(entry.Value));
    }

    public Dictionary<ItemId, InventoryItem> LoadInventory()
    {
        // Use clone constructor to loose references
        return this.inventory.ToDictionary(entry => entry.Key, entry => new InventoryItem(entry.Value));
    }
}

