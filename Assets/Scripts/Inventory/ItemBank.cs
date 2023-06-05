using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using SDD.Events;

public class ItemBank : MonoBehaviour
{
    #region Custom Inspector
    [SerializeField] private List<ItemId> itemKeys = new List<ItemId>();
    [SerializeField] private List<ItemEntry> itemValues = new List<ItemEntry>();

    [SerializeField] public GameObject Inspector_CommonParticles;
    [SerializeField] public GameObject Inspector_RareParticles;
    [SerializeField] public GameObject Inspector_LegendaryParticles;

    private bool loaded = false;

    public List<ItemId> ItemKeys => itemKeys;
    public List<ItemEntry> ItemValues => itemValues;

    public void Inspector_AddItem(ItemId id, ItemEntry entry)
    {
        if (!itemKeys.Contains(id))
        {
            itemKeys.Add(id);
            itemValues.Add(entry);
        }
    }

    public void Inspector_RemoveItem(ItemId id)
    {
        if (itemKeys.Contains(id))
        {
            int index = itemKeys.IndexOf(id);
            itemKeys.RemoveAt(index);
            itemValues.RemoveAt(index);
        }
    }

    private void Inspector_LoadValues()
    {
        for (int i = 0; i < itemKeys.Count; i++)
        {
            items[itemKeys[i]] = itemValues[i];
        }
        commonParticles = Inspector_CommonParticles;
        rareParticles = Inspector_RareParticles;
        legendaryParticles = Inspector_LegendaryParticles;
    }
    #endregion

    private static ItemBank m_Instance;
    public static ItemBank Instance => m_Instance;

    private Dictionary<ItemId, ItemEntry> items = new Dictionary<ItemId, ItemEntry>();

    private static GameObject commonParticles;
    private static GameObject rareParticles;
    private static GameObject legendaryParticles;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!loaded)
        {
            Inspector_LoadValues();
        }
    }

    public static string GetName(ItemId id)
    {
        return Instance.items[id].name;
    }

    public static Sprite GetIcon(ItemId id)
    {
        return Instance.items[id].icon;
    }

    public static GameObject GetHeldPrefab(ItemId id)
    {
        return Instance.items[id].heldPrefab;
    }

    public static GameObject GetDroppedPrefab(ItemId id)
    {
        return Instance.items[id].droppedPrefab;
    }

    public static GameObject GetDroppedItem(ItemId id)
    {
        GameObject prefab = GetDroppedPrefab(id);
        DroppedItem droppedItem = prefab.AddComponent<DroppedItem>();
        droppedItem.Init(id);
        return prefab;
    }

    public static bool IsConsumable(ItemId id)
    {
        return Instance.items[id].isConsumable;
    }

    public static ItemRarity GetRarity(ItemId id)
    {
        return Instance.items[id].rarity;
    }

    public static GameObject GetDroppedParticles(ItemId id)
    {
        ItemRarity rarity = GetRarity(id);
        switch (rarity)
        {
            case ItemRarity.Common: return commonParticles;
            case ItemRarity.Rare: return rareParticles;
            case ItemRarity.Legendary: return legendaryParticles;
            default: return commonParticles;
        }
    }
}



