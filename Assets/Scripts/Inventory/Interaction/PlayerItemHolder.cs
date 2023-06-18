using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : MonoBehaviour
{
    [SerializeField] private GameObject itemHolder;
    private GameObject heldPrefab;
    private GameObject heldGameObject;

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
        EventManager.Instance.AddListener<UseItemEvent>(UseItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerHeldItemUpdateEvent>(UpdateItemInHand);
        EventManager.Instance.RemoveListener<UseItemEvent>(UseItem);
    }

    private void UpdateItemInHand(PlayerHeldItemUpdateEvent e)
    {
        DestroyItemsInHand();
        StopHoldingItem();
        if (e.itemId == null) return;

        heldPrefab = ItemBank.GetHeldPrefab(e.itemId.Value);
        heldGameObject = Instantiate(heldPrefab, itemHolder.transform);
        StartHoldingItem();
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

    private void UseItem(UseItemEvent e)
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (heldGameObject == null) return;

        IUseItem useItemInterface = heldGameObject.GetComponent<IUseItem>();
        if (useItemInterface != null)
        {
            if (useItemInterface.Use())
            {
                if (ItemBank.IsConsumable(e.itemId))
                {
                    EventManager.Instance.Raise(new ConsumeItemEvent { itemId = e.itemId });
                }
            }
        }
        return;
    }

    private void StopHoldingItem()
    {
        if (heldGameObject == null) return;
        IHoldingItem holdingItemInterface = heldGameObject.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnStopHolding();
    }

    private void StartHoldingItem()
    {
        if (heldGameObject == null) return;
        IHoldingItem holdingItemInterface = heldGameObject.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnHolding();
    }
}
