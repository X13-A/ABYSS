using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class InventoryManager2 : MonoBehaviour, IEventHandler
{
    private static InventoryManager2 m_Instance;
    public static InventoryManager2 Instance => m_Instance;

    [SerializeField] private List<Slot> slots;
    private Dictionary<ItemId, InventoryItem> items;


    private int activeSlot;
    public int ActiveSlot => this.activeSlot;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
            activeSlot = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }

    private int FindFreeSlot()
    {
        bool[] slotsFree = new bool[this.slots.Count];

        foreach (KeyValuePair<ItemId, InventoryItem> kvp in this.items)
        {
            InventoryItem item = kvp.Value;
            slotsFree[item.Slot] = true;
        }

        for (int i = 0; i < slotsFree.Length; i++)
        {
            if (slotsFree[i] == false) // Slot is available
            {
                return i;
            }
        }
        return -1;
    }

    public bool HasSpaceForItem(ItemId id)
    {
        if (this.items.ContainsKey(id)) return true;
        return (FindFreeSlot() != -1);
    }

    private void AddItem(ItemId id)
    {
        if (this.items.ContainsKey(id))
        {
            this.items[id].Add(1);
            return;
        }

        int freeSlot = FindFreeSlot();
        if (freeSlot != -1)
        {
            this.items.Add(id, new InventoryItem(id, freeSlot, 1));
        }
    }

    private void RemoveItem(ItemId id)
    {
        if (this.items.ContainsKey(id))
        {
            this.items[id].Remove(1);
            if (this.items[id].Count <= 0)
            {
                this.items.Remove(id);
            }
        }
    }
}
