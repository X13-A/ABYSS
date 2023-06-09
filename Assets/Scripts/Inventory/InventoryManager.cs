using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IEventHandler
{
    private static InventoryManager m_Instance;
    public static InventoryManager Instance => m_Instance;

    [SerializeField] private List<Slot> slots;
    private Dictionary<ItemId, InventoryItem> items;

    private int activeSlot;
    public int ActiveSlot => activeSlot;

    public ItemId? ActiveItem => slots[activeSlot].ItemId;

    public int SlotCount => slots.Count;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
            activeSlot = 0;
            items = new Dictionary<ItemId, InventoryItem>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        //Load data when enabled
        if (PlayerData.Instance != null)
        {
            items = PlayerData.Instance.LoadInventory();
        }

        UpdateSlots();
        SwitchSlot(new SwitchSlotEvent { slot = activeSlot });
        SubscribeEvents();
    }

    private void OnDisable()
    {
        //Save data when disabled
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.SaveInventory(items);
        }

        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ItemPickedUpEvent>(AddItem);
        EventManager.Instance.AddListener<ItemRemovedEvent>(RemoveItem);
        EventManager.Instance.AddListener<SwitchSlotEvent>(SwitchSlot);
        EventManager.Instance.AddListener<GameOverEvent>(ClearInventory);

        EventManager.Instance.AddListener<UseKeyPressedEvent>(UseItem);
        EventManager.Instance.AddListener<ConsumeItemEvent>(ConsumeItem);
        EventManager.Instance.AddListener<DropKeyPressedEvent>(DropItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(AddItem);
        EventManager.Instance.RemoveListener<ItemRemovedEvent>(RemoveItem);
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(SwitchSlot);
        EventManager.Instance.RemoveListener<GameOverEvent>(ClearInventory);

        EventManager.Instance.RemoveListener<UseKeyPressedEvent>(UseItem);
        EventManager.Instance.RemoveListener<ConsumeItemEvent>(ConsumeItem);
        EventManager.Instance.RemoveListener<DropKeyPressedEvent>(DropItem);
    }


    private void ClearInventory(GameOverEvent e)
    {
        items.Clear();
        //this.UpdateSlots();
    }

    public bool HasSpaceForItem(ItemId id)
    {
        // Returns true if the item specified can be picked up
        if (items.ContainsKey(id)) return true;
        return (FindFreeSlot() != -1);
    }

    public InventoryItem ItemInSlot(int slot)
    {
        // Returns the item in the specified slot, if existing
        foreach (KeyValuePair<ItemId, InventoryItem> kvp in items)
        {
            InventoryItem item = kvp.Value;
            if (item.Slot == slot) return item;
        }
        return null;
    }

    private int FindFreeSlot()
    {
        // Finds the first free slot in the inventory
        bool[] slotsFree = new bool[slots.Count];

        foreach (KeyValuePair<ItemId, InventoryItem> kvp in items)
        {
            InventoryItem item = kvp.Value;
            slotsFree[item.Slot] = true;
        }

        for (int i = 0; i < slotsFree.Length; i++)
        {
            if (slotsFree[i] == false)
            {
                // Slot is available
                return i;
            }
        }
        return -1;
    }
    private void AddItem(ItemPickedUpEvent e)
    {
        // Adds one item if existing already
        if (items.ContainsKey(e.itemId))
        {
            items[e.itemId].Add(1);
            UpdateSlots();
            return;
        }

        // Else, creates one item
        int freeSlot = FindFreeSlot();
        if (freeSlot != -1)
        {
            items.Add(e.itemId, new InventoryItem(e.itemId, freeSlot, 1));
            UpdateSlots();
        }
    }
    private void RemoveItem(ItemRemovedEvent e)
    {
        // Removes 1 item from inventory
        if (items.ContainsKey(e.itemId))
        {
            items[e.itemId].Remove(1);

            // If item count is at 0, deletes completely the item
            if (items[e.itemId].Count <= 0)
            {
                items.Remove(e.itemId);
            }
            UpdateSlots();
        }
    }

    private void SwitchSlot(SwitchSlotEvent e)
    {
        // Select new slot and unselect old ones
        activeSlot = e.slot;
        for (int i = 0; i < slots.Count; i++)
        {
            if (activeSlot == i)
            {
                // Select new slot
                slots[i].SetSelected(true);
            }
            else slots[i].SetSelected(false);
        }
        EventManager.Instance.Raise(new PlayerHeldItemUpdateEvent { itemId = slots[activeSlot].ItemId });
    }

    private void UseItem(UseKeyPressedEvent e)
    {
        // Uses item
        if (ActiveItem == null) return;
        EventManager.Instance.Raise(new UseItemEvent { itemId = ActiveItem.Value });
    }

    private void ConsumeItem(ConsumeItemEvent e)
    {
        EventManager.Instance.Raise(new ItemRemovedEvent { itemId = e.itemId });
        UpdateSlots();
    }

    private void DropItem(DropKeyPressedEvent e)
    {
        if (ActiveItem == null) return;
        InventoryItem item = items[(ItemId) ActiveItem];
        if (item != null)
        {
            EventManager.Instance.Raise(new ItemDroppedEvent { itemId = item.Id });
            // Calls event to start the suppression of the dropped item
            EventManager.Instance.Raise(new ItemRemovedEvent { itemId = item.Id });
        }
    }

    private void UpdateSlots()
    {
        // Updates the information in each slot according to the "items" Dict
        for (int i = 0; i < slots.Count; i++)
        {
            bool foundItem = false;
            foreach (KeyValuePair<ItemId, InventoryItem> kvp in items)
            {
                ItemId itemId = kvp.Key;
                InventoryItem inventoryItem = kvp.Value;
                if (inventoryItem.Slot == i)
                {
                    slots[i].UpdateInfo(inventoryItem);
                    foundItem = true;
                }
            }
            // Displays no information if slot does not hold anything
            if (foundItem) continue;
            slots[i].UpdateInfo(null);
        }

        EventManager.Instance.Raise(new PlayerHeldItemUpdateEvent { itemId = slots[activeSlot].ItemId });
    }
}
