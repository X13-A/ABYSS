using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentItemMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private Coroutine waitCoroutine;
    private Coroutine fadeCoroutine;

    private int collidingItems;
    private int collidingChests;

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
        EventManager.Instance.AddListener<SwitchSlotEvent>(this.DisplaySelectedName);
        EventManager.Instance.AddListener<ItemPickedUpEvent>(this.DisplayPickedUpName);
        EventManager.Instance.AddListener<UpdateCollidingItemsEvent>(this.HandleCollidingItemsUpdate);
        EventManager.Instance.AddListener<UpdateCollidingChestsEvent>(this.HandleCollidingChestsUpdate);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(this.DisplaySelectedName);
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(this.DisplayPickedUpName);
        EventManager.Instance.RemoveListener<UpdateCollidingItemsEvent>(this.HandleCollidingItemsUpdate);
        EventManager.Instance.RemoveListener<UpdateCollidingChestsEvent>(this.HandleCollidingChestsUpdate);
    }

    private void HandleCollidingChestsUpdate(UpdateCollidingChestsEvent e)
    {
        this.collidingChests = e.chests.Count;
        this.HandleCollidingObjectsUpdate();
    }

    private void HandleCollidingObjectsUpdate()
    {
        if (this.collidingItems > 0 || this.collidingChests > 0)
        {
            if (this.fadeCoroutine != null)
            {
                StopCoroutine(this.fadeCoroutine);
                this.fadeCoroutine = null;
            }
            this.text.text = "";
            this.text.enabled = false;
        }
    }

    private void HandleCollidingItemsUpdate(UpdateCollidingItemsEvent e)
    {
        this.collidingItems = e.items.Count;
        HandleCollidingObjectsUpdate();
    }

    private void DisplayPickedUpName(ItemPickedUpEvent e)
    {
        if (InventoryManager.Instance.ActiveItem == e.itemId)
        {
            this.DisplayItemName(e.itemId);
        }
    }


    private void DisplaySelectedName(SwitchSlotEvent e)
    {
        if (this.collidingItems > 0) return;
        ItemId? item = InventoryManager.Instance.ActiveItem;
        EventManager.Instance.Raise(new SelectedItemEvent { itemId = item });
        this.DisplayItemName(item);
    }

    private void DisplayItemName(ItemId? item)
    {
        Color startColor = this.text.material.color;
        startColor.a = 1f;
        this.text.material.color = startColor;

        if (this.fadeCoroutine != null)
        {
            StopCoroutine(this.fadeCoroutine);;
            this.fadeCoroutine = null;
        }

        if (item == null)
        {
            this.text.text = "";
            this.text.enabled = false;
            return;
        }

        string name = ItemBank.GetName(item.Value);
        this.text.text = $"{name}";
        this.text.enabled = true;

        this.fadeCoroutine = StartCoroutine(CoroutineUtil.FadeUITextTo(this.text, 1f, 0f, () =>
        {
            this.text.text = "";
            this.text.enabled = false;
        }));
    }
}
