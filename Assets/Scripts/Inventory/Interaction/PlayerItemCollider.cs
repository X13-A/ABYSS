using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollider : MonoBehaviour, IEventHandler, IPlayerCollider
{
    private HashSet<DroppedItem> itemsToPickup = new HashSet<DroppedItem>();

    private void OnEnable()
    {
        this.SubscribeEvents();
    }

    private void OnDisable()
    {
        this.UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PickupKeyPressedEvent>(this.Pickup);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PickupKeyPressedEvent>(this.Pickup);
    }

    private void Pickup(PickupKeyPressedEvent e)
    {
        if (itemsToPickup.Count == 0) return;
        HashSet<DroppedItem> pickedupItems = new HashSet<DroppedItem>();
        foreach (DroppedItem item in itemsToPickup)
        {
            if (item.Pickup())
            {
                pickedupItems.Add(item);
            }
        }
        itemsToPickup.ExceptWith(pickedupItems);
        EventManager.Instance.Raise(new UpdateCollidingItemsEvent { items = itemsToPickup });
    }

    private void OnTriggerEnter(Collider other)
    {
        DroppedItem item = other.GetComponent<DroppedItem>();
        if (item != null)
        {
            itemsToPickup.Add(item);
            EventManager.Instance.Raise(new UpdateCollidingItemsEvent { items = itemsToPickup });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DroppedItem item = other.GetComponent<DroppedItem>();
        if (item != null)
        {
            itemsToPickup.Remove(item);
            EventManager.Instance.Raise(new UpdateCollidingItemsEvent { items = itemsToPickup });
        }
    }
}
