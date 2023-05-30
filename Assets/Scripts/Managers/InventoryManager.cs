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

    public int ActiveSlot => activeSlot;

    private Dictionary<string, IInventoryItem> mItems = new Dictionary<string, IInventoryItem>();
    private Dictionary<string, int> mItemsCount = new Dictionary<string, int>();

    public Dictionary<string, IInventoryItem> Items => mItems;
    public Dictionary<string, int> ItemsCount => mItemsCount;

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
        EventManager.Instance.AddListener<SwitchSlot>(setActiveSlot);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SwitchSlot>(setActiveSlot);
    }

    public void AddItem(IInventoryItem item)
    {
        if (mItems.Count < SLOTS || mItems.ContainsKey(item.Name))
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;

                if (mItems.ContainsKey(item.Name))
                {
                    Debug.Log(mItemsCount[item.Name]);
                    mItemsCount[item.Name] += 1;
                }
                else
                {
                    mItems.Add(item.Name, item);
                    mItemsCount.Add(item.Name, 1);
                }

                item.OnPickup();

                if (item != null)
                {
                    EventManager.Instance.Raise(new ItemAddedEvent
                    {
                        item = item,
                        count = mItemsCount[item.Name]
                    });
                    EventManager.Instance.Raise(new SwitchSlot
                    {
                        slot = InventoryManager.Instance.ActiveSlot
                    });
                }
            }
        }
    }

    public void RemovedItem(IInventoryItem item)
    {
        if (mItems.ContainsKey(item.Name))
        {
            mItemsCount[item.Name] -= 1;
            if (mItemsCount[item.Name] <= 0)
            {
                mItemsCount.Remove(item.Name);
                mItems.Remove(item.Name);
            }

            item.OnDrop();

            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (item != null)
            {
                EventManager.Instance.Raise(new ItemRemovedEvent
                {
                    item = item,
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
        activeSlot = e.slot;
        GameObject activeGameObject = inventoryPanel.transform.GetChild(activeSlot).GetComponent<gameObjectSlot>().gameObjectInSlot;
        if (activeGameObject != null)
        {
            newPlayerMode = activeGameObject.GetComponent<InventoryItemBase>().PlayerModeObject;
            BlockDamage scriptBlockDamage = activeGameObject.GetComponent<BlockDamage>();
            if (scriptBlockDamage != null) scriptBlockDamage.Health = 10;
        }
        else
        {
            newPlayerMode = PlayerMode.UNARMED;
        }
        
        EventManager.Instance.Raise(new UpdateObjectInhand { objectInSlot = activeGameObject });
        EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = newPlayerMode});
    }
}
