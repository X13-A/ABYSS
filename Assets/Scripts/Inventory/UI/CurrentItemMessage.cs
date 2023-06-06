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
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(this.DisplaySelectedName);
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(this.DisplayPickedUpName);
        EventManager.Instance.RemoveListener<UpdateCollidingItemsEvent>(this.HandleCollidingItemsUpdate);
    }

    private void HandleCollidingItemsUpdate(UpdateCollidingItemsEvent e)
    {
        this.collidingItems = e.items.Count;
        if (this.collidingItems > 0)
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
