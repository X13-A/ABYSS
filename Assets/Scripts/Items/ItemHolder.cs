using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private GameObject heldItem;
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
        EventManager.Instance.AddListener<HeldGameObjectEvent>(UpdateHeldItem);
        EventManager.Instance.AddListener<UseItemEvent>(UseItem);
        
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<HeldGameObjectEvent>(UpdateHeldItem);
        EventManager.Instance.RemoveListener<UseItemEvent>(UseItem);
    }

    private void UpdateHeldItem(HeldGameObjectEvent e)
    {
        StopHoldingItem();
        if (e.heldGameObject == null) return;
        heldItem = e.heldGameObject;
        HoldingItem();
    }

    private void UseItem(UseItemEvent e)
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (heldItem == null) return;
        
        IUseItem useItemInterface = heldItem.GetComponent<IUseItem>();
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
        if (heldItem == null) return;
        IHoldingItem holdingItemInterface = heldItem.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnStopHolding();
    }

    private void HoldingItem()
    {
        IHoldingItem holdingItemInterface = heldItem.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnHolding();
    }
}
