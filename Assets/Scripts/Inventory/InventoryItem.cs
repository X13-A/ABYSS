using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    private ItemId id;
    private int slot;
    private int count;

    public int Slot => slot;
    public int Count => count;
    public ItemId Id => id;


    public InventoryItem(ItemId id, int slot, int count)
    {
        this.id = id;
        this.count = count;
        this.slot = slot;
    }

    /// <summary>
    /// Clone constructor
    /// </summary>
    /// <param name="item">The item to clone</param>
    public InventoryItem(InventoryItem item)
    {
        this.id = item.id;
        this.count = item.count;
        this.slot = item.slot;
    }

    public void Add(int n)
    {
        count += n;
    }

    public void Remove(int n)
    {
        count -= n;
    }
}
