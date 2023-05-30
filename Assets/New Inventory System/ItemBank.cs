using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBank : MonoBehaviour
{
    #region Custom Inspector
    [SerializeField] private List<ItemId> itemKeys = new List<ItemId>();
    [SerializeField] private List<ItemEntry> itemValues = new List<ItemEntry>();
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
    }
    #endregion

    private static ItemBank m_Instance;
    public static ItemBank Instance => m_Instance;

    private Dictionary<ItemId, ItemEntry> items = new Dictionary<ItemId, ItemEntry>();
    
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

    public static Image GetIcon(ItemId id)
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

    public static void UseItem(ItemId id)
    {
        switch (id)
        {
            case ItemId.Map: return;
            case ItemId.Pickaxe: return;
            case ItemId.Sword: return;
            case ItemId.Wand: return;
        }
    }
}



