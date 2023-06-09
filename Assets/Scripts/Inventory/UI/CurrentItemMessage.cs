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
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<SwitchSlotEvent>(DisplaySelectedName);
        EventManager.Instance.AddListener<ItemPickedUpEvent>(DisplayPickedUpName);
        EventManager.Instance.AddListener<UpdateCollidingItemsEvent>(HandleCollidingItemsUpdate);
        EventManager.Instance.AddListener<UpdateCollidingChestsEvent>(HandleCollidingChestsUpdate);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(DisplaySelectedName);
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(DisplayPickedUpName);
        EventManager.Instance.RemoveListener<UpdateCollidingItemsEvent>(HandleCollidingItemsUpdate);
        EventManager.Instance.RemoveListener<UpdateCollidingChestsEvent>(HandleCollidingChestsUpdate);
    }

    private void HandleCollidingChestsUpdate(UpdateCollidingChestsEvent e)
    {
        collidingChests = e.chests.Count;
        HandleCollidingObjectsUpdate();
    }

    private void HandleCollidingObjectsUpdate()
    {
        if (collidingItems > 0 || collidingChests > 0)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            text.text = "";
            text.enabled = false;
        }
    }

    private void HandleCollidingItemsUpdate(UpdateCollidingItemsEvent e)
    {
        collidingItems = e.items.Count;
        HandleCollidingObjectsUpdate();
    }

    private void DisplayPickedUpName(ItemPickedUpEvent e)
    {
        if (InventoryManager.Instance.ActiveItem == e.itemId)
        {
            DisplayItemName(e.itemId);
        }
    }


    private void DisplaySelectedName(SwitchSlotEvent e)
    {
        if (collidingItems > 0) return;
        ItemId? item = InventoryManager.Instance.ActiveItem;
        EventManager.Instance.Raise(new SelectedItemEvent { itemId = item });
        DisplayItemName(item);
    }

    private void DisplayItemName(ItemId? item)
    {
        Color startColor = text.material.color;
        startColor.a = 1f;
        text.material.color = startColor;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);;
            fadeCoroutine = null;
        }

        if (item == null)
        {
            text.text = "";
            text.enabled = false;
            return;
        }

        string name = ItemBank.GetName(item.Value);
        text.text = $"{name}";
        text.enabled = true;

        fadeCoroutine = StartCoroutine(CoroutineUtil.FadeUITextTo(text, 1f, 0f, () =>
        {
            text.text = "";
            text.enabled = false;
        }));
    }
}
