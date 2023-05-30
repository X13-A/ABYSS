using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IEventHandler
{
    public static InventoryManager m_Instance;
    public static InventoryManager Instance => m_Instance;

    private const int SLOTS = 9;

    [SerializeField] private GameObject inventoryPanel;

    private int activeSlot;

    public int ActiveSlot => this.activeSlot;

    // HACK: Use restrictive methods instead
    public Dictionary<string, GameObject> Items => PlayerData.Instance.Items;
    public Dictionary<string, int> ItemsCount => PlayerData.Instance.ItemsCount;
    public Dictionary<string, int> ItemsSlot => PlayerData.Instance.ItemsSlot;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
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
        EventManager.Instance.AddListener<SwitchSlot>(this.setActiveSlot);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SwitchSlot>(this.setActiveSlot);
    }

    private int FindFreeSlot()
    {
        bool[] slots = new bool[SLOTS];

        foreach (KeyValuePair<string, GameObject> kvp in this.Items)
        {
            string name = kvp.Key;
            int slot = this.ItemsSlot[name];
            slots[slot] = true;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == false) // Slot is available
            {
                return i;
            }
        }
        return -1;
    }

    public void AddItem(GameObject item)
    {
        IInventoryItem inventoryItem = item.GetComponent<IInventoryItem>();
        if (this.Items.Count < SLOTS || this.Items.ContainsKey(inventoryItem.Name))
        {
            Collider collider = item.GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;

                if (this.Items.ContainsKey(inventoryItem.Name))
                {
                    ItemsCount[inventoryItem.Name] += 1;
                }
                else
                {
                    this.ItemsSlot.Add(inventoryItem.Name, FindFreeSlot());
                    this.Items.Add(inventoryItem.Name, item);
                    this.ItemsCount.Add(inventoryItem.Name, 1);
                }

                inventoryItem.OnPickup();

                if (item != null)
                {
                    EventManager.Instance.Raise(new ItemAddedEvent
                    {
                        item = inventoryItem,
                        count = this.ItemsCount[inventoryItem.Name]
                    });
                    EventManager.Instance.Raise(new SwitchSlot
                    {
                        slot = ActiveSlot
                    });
                }
            }
        }
    }

    public void RemovedItem(GameObject item)
    {
        IInventoryItem inventoryItem = item.GetComponent<IInventoryItem>();

        if (this.Items.ContainsKey(inventoryItem.Name))
        {
            ItemsCount[inventoryItem.Name] -= 1;
            if (this.ItemsCount[inventoryItem.Name] <= 0)
            {
                this.ItemsCount.Remove(inventoryItem.Name);
                this.ItemsSlot.Remove(inventoryItem.Name);
                this.Items.Remove(inventoryItem.Name);
            }

            inventoryItem.OnDrop();

            Collider collider = item.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (item != null)
            {
                EventManager.Instance.Raise(new ItemRemovedEvent
                {
                    item = inventoryItem,
                    count = 0
                });
            }
        }
    }

    internal void UseItem(IInventoryItem item)
    {
        if (item != null)
        {
            EventManager.Instance.Raise(new ItemUsedEvent
            {
                item = item,
            });
        }
    }

    private void setActiveSlot(SwitchSlot e)
    {
        PlayerMode newPlayerMode;
        this.activeSlot = e.slot;
        GameObject activeGameObject = inventoryPanel.transform.GetChild(this.activeSlot).GetComponent<gameObjectSlot>().gameObjectInSlot;
        bool mapSelected = false;

        if (activeGameObject != null)
        {
            newPlayerMode = activeGameObject.GetComponent<InventoryItemBase>().PlayerModeObject;
            BlockDamage scriptBlockDamage = activeGameObject.GetComponent<BlockDamage>();
            if (scriptBlockDamage != null) scriptBlockDamage.Health = 10;

            // Display map if held
            if (activeGameObject.GetComponent<MinimapItem>() != null)
            {
                mapSelected = true;
            }
        }
        else
        {
            newPlayerMode = PlayerMode.UNARMED;
        }


        if (mapSelected) EventManager.Instance.Raise(new ToggleMapEvent { value = true });
        else  EventManager.Instance.Raise(new ToggleMapEvent { value = false });

        EventManager.Instance.Raise(new UpdateObjectInhand { objectInSlot = activeGameObject });
        EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = newPlayerMode});

    }
}
