using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChestOpener : MonoBehaviour, IEventHandler, IPlayerCollider
{
    private HashSet<Chest> collidingChests = new HashSet<Chest>();

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
        EventManager.Instance.AddListener<PickupKeyPressedEvent>(OpenChests);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PickupKeyPressedEvent>(OpenChests);
    }

    private void OpenChests(PickupKeyPressedEvent e)
    {
        if (collidingChests.Count == 0) return;
        foreach (Chest chest in collidingChests)
        {
            chest.Open();
        }
        collidingChests.Clear();
        EventManager.Instance.Raise(new UpdateCollidingChestsEvent { chests = collidingChests });
    }

    private void OnTriggerEnter(Collider other)
    {
        Chest chest = other.GetComponent<Chest>();
        if (chest == null) return;
        if (chest.Opened == true) return;
        collidingChests.Add(chest);
        EventManager.Instance.Raise(new UpdateCollidingChestsEvent { chests = collidingChests });
    }

    private void OnTriggerExit(Collider other)
    {
        Chest chest = other.GetComponent<Chest>();
        if (chest == null) return;
        collidingChests.Remove(chest);
        EventManager.Instance.Raise(new UpdateCollidingChestsEvent { chests = collidingChests });
    }
}
