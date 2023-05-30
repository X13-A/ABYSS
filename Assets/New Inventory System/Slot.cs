using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour, IEventHandler
{
    [SerializeField] private GameObject slot;
    private ItemId? itemId = null;

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
        EventManager.Instance.AddListener<ItemAddedEvent>(this.HandleUpdate);
        EventManager.Instance.AddListener<ItemRemovedEvent>(this.HandleUpdate);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemAddedEvent>(this.HandleUpdate);
        EventManager.Instance.RemoveListener<ItemRemovedEvent>(this.HandleUpdate);
    }

    private void HandleUpdate(ItemAddedEvent)
    {
        this.UpdateInfo();
    }
    private void HandleUpdate(ItemRemovedEvent)
    {
        this.UpdateInfo();
    }

    private void UpdateInfo()
    {
        // TODO: Set id from InventoryManager
        // TODO: Set count from InventoryManager
        // TODO: Set icon from ItemBank
    }
}
