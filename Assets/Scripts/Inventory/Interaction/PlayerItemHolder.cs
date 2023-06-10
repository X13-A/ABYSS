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
        SubscribeEvents();

        // HACK, because this script is not up when the event is fired
        UpdateItemInHand(new PlayerHeldItemUpdateEvent { itemId = InventoryManager.Instance ? InventoryManager.Instance.ActiveItem : null });
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerHeldItemUpdateEvent>(UpdateItemInHand);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerHeldItemUpdateEvent>(UpdateItemInHand);
    }

    private void UpdateItemInHand(PlayerHeldItemUpdateEvent e)
    {
        DestroyItemsInHand();
        if (e.itemId == null)
        {
            EventManager.Instance.Raise(new HeldGameObjectEvent { heldGameObject = null });
            return;
        }
        heldPrefab = ItemBank.GetHeldPrefab((ItemId) e.itemId);
        GameObject heldItem = Instantiate(heldPrefab, itemHolder.transform);
        EventManager.Instance.Raise(new HeldGameObjectEvent { heldGameObject = heldItem });
    }

    private void DestroyItemsInHand()
    {
        int count = itemHolder.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            GameObject item = itemHolder.transform.GetChild(i).gameObject;
            Destroy(item);
        }
    }
}
