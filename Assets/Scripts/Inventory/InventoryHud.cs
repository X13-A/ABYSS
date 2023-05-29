using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : MonoBehaviour, IEventHandler
{
    [SerializeField] private GameObject pickUpMessagePanel;
    private HashSet<IInventoryItem> collidingItems = new HashSet<IInventoryItem>();
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
        EventManager.Instance.AddListener<ItemAddedEvent>(this.InventoryScript_ItemAdded);
        EventManager.Instance.AddListener<ItemRemovedEvent>(this.InventoryScript_ItemRemoved);
        EventManager.Instance.AddListener<ItemCollideWithPlayerEvent>(this.addPickableItem);
        EventManager.Instance.AddListener<ItemEndCollideWithPlayerEvent>(this.removePickableItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemAddedEvent>(this.InventoryScript_ItemAdded);
        EventManager.Instance.RemoveListener<ItemRemovedEvent>(this.InventoryScript_ItemRemoved);
        EventManager.Instance.RemoveListener<ItemCollideWithPlayerEvent>(this.addPickableItem);
        EventManager.Instance.RemoveListener<ItemEndCollideWithPlayerEvent>(this.removePickableItem);
    }

    private void addPickableItem(ItemCollideWithPlayerEvent e)
    {
        this.collidingItems.Add(e.item);
        pickUpMessagePanel.SetActive(true);
    }

    private void removePickableItem(ItemEndCollideWithPlayerEvent e)
    {
        this.collidingItems.Remove(e.item);
        if (this.collidingItems.Count == 0)
        {
            pickUpMessagePanel.SetActive(false);
        }
    }

    private void InventoryScript_ItemAdded(ItemAddedEvent e)
    {
        pickUpMessagePanel.SetActive(false);
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform countItem = imageTransform.GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();
            if (image.enabled && image.sprite == e.item.Image)
            {
                textCountItem.text = e.count.ToString();
                return;
            }
        }
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform countItem = imageTransform.GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (!image.enabled)
            {
                image.enabled = true;
                textCountItem.enabled = true;
                image.sprite = e.item.Image;
                itemDragHandler.Item = e.item;
                textCountItem.text = e.count.ToString();
                break;
            }
        }
    }

    private void InventoryScript_ItemRemoved(ItemRemovedEvent e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform countItem = imageTransform.GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (itemDragHandler.Item.Equals(e.item))
            {
                image.enabled = false;
                image.sprite = null;
                textCountItem.enabled = false;
                //itemDragHandler.Item = null;

                break;
            }
        }
    }
}
