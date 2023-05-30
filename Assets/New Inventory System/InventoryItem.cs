using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
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

    public void Add(int n)
    {
        count += n;
    }

    public void Remove(int n)
    {
        count -= n;
    }
    public void Use()
    {
        ItemBank.UseItem(this.id);
    }

    public void Drop()
    {

    }
}
