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

    private Dictionary<string, IInventoryItem> mItems = new Dictionary<string, IInventoryItem>();
    private Dictionary<string, int> mItemsCount = new Dictionary<string, int>();

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

    }

    public void UnsubscribeEvents()
    {

    }

    public void AddItem(IInventoryItem item)
    {
        if (this.mItems.Count < SLOTS || this.mItems.ContainsKey(item.Name))
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                collider.enabled = false;

                if (this.mItems.ContainsKey(item.Name))
                {
                    Debug.Log(mItemsCount[item.Name]);
                    mItemsCount[item.Name] += 1;
                }
                else
                {
                    this.mItems.Add(item.Name, item);
                    this.mItemsCount.Add(item.Name, 1);
                }

                item.OnPickup();

                if (item != null)
                {
                    EventManager.Instance.Raise(new ItemAddedEvent
                    {
                        item = item,
                        count = this.mItemsCount[item.Name]
                    });
                }
            }
        }
    }

    public void RemovedItem(IInventoryItem item)
    {
        if (this.mItems.ContainsKey(item.Name))
        {
            mItemsCount[item.Name] -= 1;
            if (this.mItemsCount[item.Name] <= 0)
            {
                this.mItemsCount.Remove(item.Name);
                this.mItems.Remove(item.Name);
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
}
