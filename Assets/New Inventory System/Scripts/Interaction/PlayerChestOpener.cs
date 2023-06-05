using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChestOpener : MonoBehaviour, IEventHandler, IPlayerCollider
{
    private HashSet<Chest> collidingChests = new HashSet<Chest>();

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
        EventManager.Instance.AddListener<PickupKeyPressedEvent>(this.OpenChests);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PickupKeyPressedEvent>(this.OpenChests);
    }

    private void OpenChests(PickupKeyPressedEvent e)
    {
        if (collidingChests.Count == 0) return;
        HashSet<Chest> pickedupItems = new HashSet<Chest>();
        foreach (Chest chest in collidingChests)
        {
            chest.Open();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Chest chest = other.GetComponent<Chest>();
        if (chest == null) return;
        collidingChests.Add(chest);        
    }

    private void OnTriggerExit(Collider other)
    {
        Chest chest = other.GetComponent<Chest>();
        if (chest == null) return;
        collidingChests.Remove(chest);
    }
}
