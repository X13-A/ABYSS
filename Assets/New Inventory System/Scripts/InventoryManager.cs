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
    public int ActiveSlot => this.activeSlot;

    public ItemId? ActiveItem => this.slots[activeSlot].ItemId;

    public int SlotCount => this.slots.Count;

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
        //if (PlayerData.Instance != null)
        //{
        //    this.items = PlayerData.Instance.LoadInventory();
        //}

        this.UpdateSlots();
        this.SwitchSlot(new SwitchSlotEvent { slot = activeSlot });
        SubscribeEvents();
    }

    private void OnDisable()
    {
        //Save data when disabled
        //if (PlayerData.Instance != null)
        //{
        //    PlayerData.Instance.SaveInventory(this.items);
        //}

        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ItemPickedUpEvent>(this.AddItem);
        EventManager.Instance.AddListener<ItemRemovedEvent>(this.RemoveItem);
        EventManager.Instance.AddListener<SwitchSlotEvent>(this.SwitchSlot);

        EventManager.Instance.AddListener<UseKeyPressedEvent>(this.UseItem);
        EventManager.Instance.AddListener<DropKeyPressedEvent>(this.DropItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(this.AddItem);
        EventManager.Instance.RemoveListener<ItemRemovedEvent>(this.RemoveItem);
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(this.SwitchSlot);

        EventManager.Instance.RemoveListener<UseKeyPressedEvent>(this.UseItem);
        EventManager.Instance.RemoveListener<DropKeyPressedEvent>(this.DropItem);
    }
    public bool HasSpaceForItem(ItemId id)
    {
        // Returns true if the item specified can be picked up
        if (this.items.ContainsKey(id)) return true;
        return (FindFreeSlot() != -1);
    }

    public InventoryItem ItemInSlot(int slot)
    {
        // Returns the item in the specified slot, if existing
        foreach (KeyValuePair<ItemId, InventoryItem> kvp in this.items)
        {
            InventoryItem item = kvp.Value;
            if (item.Slot == slot) return item;
        }
        return null;
    }

    private int FindFreeSlot()
    {
        // Finds the first free slot in the inventory
        bool[] slotsFree = new bool[this.slots.Count];

        foreach (KeyValuePair<ItemId, InventoryItem> kvp in this.items)
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
        if (this.items.ContainsKey(e.itemId))
        {
            this.items[e.itemId].Add(1);
            this.UpdateSlots();
            return;
        }

        // Else, creates one item
        int freeSlot = FindFreeSlot();
        if (freeSlot != -1)
        {
            // Trigger
            if (freeSlot == this.ActiveSlot) ItemBank.OnHoldItem(e.itemId);
            this.items.Add(e.itemId, new InventoryItem(e.itemId, freeSlot, 1));
            this.UpdateSlots();
        }
    }
    private void RemoveItem(ItemRemovedEvent e)
    {
        // Removes 1 item from inventory
        if (this.items.ContainsKey(e.itemId))
        {
            this.items[e.itemId].Remove(1);

            // If item count is at 0, deletes completely the item
            if (this.items[e.itemId].Count <= 0)
            {
                // Triggers the on stop holding event if the item is removed
                if (this.ActiveItem != null && this.ActiveItem == e.itemId)
                {
                    ItemBank.OnStopHoldingItem(e.itemId);
                }

                this.items.Remove(e.itemId);
            }
            this.UpdateSlots();
        }
    }

    private void SwitchSlot(SwitchSlotEvent e)
    {
        // Trigger action when stopping holding the item
        if (this.activeSlot != e.slot && this.ActiveItem != null)
        {
            ItemBank.OnStopHoldingItem(this.ActiveItem.Value);
        }

        // Select new slot and unselect old ones
        this.activeSlot = e.slot;
        for (int i = 0; i < this.slots.Count; i++)
        {
            if (this.activeSlot == i)
            {
                // Select new slot
                this.slots[i].SetSelected(true);

                // Trigger action when starting holding the item
                if (this.ActiveItem != null)
                {
                    ItemBank.OnHoldItem(this.ActiveItem.Value);
                }
            }
            else this.slots[i].SetSelected(false);
        }
        EventManager.Instance.Raise(new PlayerHeldItemUpdateEvent { itemId = this.slots[this.activeSlot].ItemId });
    }

    private void UseItem(UseKeyPressedEvent e)
    {
        // Uses item
        if (this.ActiveItem == null) return;
        ItemBank.UseItem((ItemId) this.ActiveItem);

        // Consumes item if necessary
        if (ItemBank.IsConsumable((ItemId) this.ActiveItem))
        {
            EventManager.Instance.Raise(new ItemRemovedEvent { itemId = (ItemId) this.ActiveItem });
            this.UpdateSlots();
        }
    }

    private void DropItem(DropKeyPressedEvent e)
    {
        if (this.ActiveItem == null) return;
        InventoryItem item = this.items[(ItemId) this.ActiveItem];
        if (item != null)
        {
            // This event is useless for now but could be used for sound effects
            EventManager.Instance.Raise(new ItemDroppedEvent { itemId = item.Id });
            // Calls event to start the suppression of the dropped item
            EventManager.Instance.Raise(new ItemRemovedEvent { itemId = item.Id });
        }
    }

    private void UpdateSlots()
    {
        // Updates the information in each slot according to the "items" Dict
        for (int i = 0; i < this.slots.Count; i++)
        {
            bool foundItem = false;
            foreach (KeyValuePair<ItemId, InventoryItem> kvp in this.items)
            {
                ItemId itemId = kvp.Key;
                InventoryItem inventoryItem = kvp.Value;
                if (inventoryItem.Slot == i)
                {
                    this.slots[i].UpdateInfo(inventoryItem);
                    foundItem = true;
                }
            }
            // Displays no information if slot does not hold anything
            if (foundItem) continue;
            this.slots[i].UpdateInfo(null);
        }

        EventManager.Instance.Raise(new PlayerHeldItemUpdateEvent { itemId = this.slots[this.activeSlot].ItemId });
    }
}
