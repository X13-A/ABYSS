using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour, IPlayerCollider
{
    private HashSet<IInventoryItem> mItemsToPickup = new HashSet<IInventoryItem>();

    private void Update()
    {
        if (mItemsToPickup.Count != 0 && Input.GetKey(KeyCode.F))
        {
            foreach (IInventoryItem item in mItemsToPickup)
            {
                InventoryManager.Instance.AddItem((item as MonoBehaviour).gameObject);
                item.OnPickup();
                EventManager.Instance.Raise(new ItemEndCollideWithPlayerEvent { item = item });
            }
            mItemsToPickup.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            mItemsToPickup.Add(item);
            EventManager.Instance.Raise(new ItemCollideWithPlayerEvent { item = item });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            mItemsToPickup.Remove(item);
            EventManager.Instance.Raise(new ItemEndCollideWithPlayerEvent { item = item });
        }
    }
}

