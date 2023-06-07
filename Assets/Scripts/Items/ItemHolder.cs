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
        this.heldItem = e.heldGameObject;
        HoldingItem();
    }

    private void UseItem(UseItemEvent e)
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (heldItem == null) return;
        IUseItem useItemInterface = heldItem.GetComponent<IUseItem>();
        if (useItemInterface != null) useItemInterface.Use();
    }

    private void StopHoldingItem()
    {
        if (heldItem == null) return;
        IHoldingItem holdingItemInterface = this.heldItem.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnStopHolding();
    }

    private void HoldingItem()
    {
        IHoldingItem holdingItemInterface = this.heldItem.GetComponent<IHoldingItem>();
        if (holdingItemInterface != null) holdingItemInterface.OnHolding();
    }
}
