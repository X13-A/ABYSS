using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 9;

    private Dictionary<string, IInventoryItem> mItems = new Dictionary<string, IInventoryItem>();
    private Dictionary<string, int> mItemsCount = new Dictionary<string, int>();

    public event EventHandler<InventoryEventArgs> ItemAdded;

    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public event EventHandler<InventoryEventArgs> ItemUsed;

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

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item, this.mItemsCount[item.Name]));
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

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item, 0));
            }
        }
    }

    internal void UseItem(IInventoryItem item)
    {
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item, this.mItemsCount[item.Name]));
        }
    }
}
