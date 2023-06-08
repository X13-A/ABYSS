using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : MonoBehaviour
{
    [SerializeField] private GameObject itemHolder;
    private GameObject heldPrefab;

    private void OnEnable()
    {
        this.SubscribeEvents();

        // HACK, because this script is not up when the event is fired
        this.UpdateItemInHand(new PlayerHeldItemUpdateEvent { itemId = InventoryManager.Instance ? InventoryManager.Instance.ActiveItem : null });
    }

    private void OnDisable()
    {
        this.UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerHeldItemUpdateEvent>(this.UpdateItemInHand);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerHeldItemUpdateEvent>(this.UpdateItemInHand);
    }

    private void UpdateItemInHand(PlayerHeldItemUpdateEvent e)
    {
        DestroyItemsInHand();
        if (e.itemId == null)
        {
            EventManager.Instance.Raise(new HeldGameObjectEvent { heldGameObject = null });
            return;
        }
        this.heldPrefab = ItemBank.GetHeldPrefab((ItemId) e.itemId);
        GameObject heldItem = Instantiate(this.heldPrefab, itemHolder.transform);
        EventManager.Instance.Raise(new HeldGameObjectEvent { heldGameObject = heldItem });
    }

    private void DestroyItemsInHand()
    {
        int count = this.itemHolder.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            GameObject item = this.itemHolder.transform.GetChild(i).gameObject;
            Destroy(item);
        }
    }
}
