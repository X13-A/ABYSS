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
        EventManager.Instance.AddListener<ItemAddedEvent>(InventoryScript_ItemAdded);
        EventManager.Instance.AddListener<ItemRemovedEvent>(InventoryScript_ItemRemoved);
        EventManager.Instance.AddListener<ItemCollideWithPlayerEvent>(addPickableItem);
        EventManager.Instance.AddListener<ItemEndCollideWithPlayerEvent>(removePickableItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemAddedEvent>(InventoryScript_ItemAdded);
        EventManager.Instance.RemoveListener<ItemRemovedEvent>(InventoryScript_ItemRemoved);
        EventManager.Instance.RemoveListener<ItemCollideWithPlayerEvent>(addPickableItem);
        EventManager.Instance.RemoveListener<ItemEndCollideWithPlayerEvent>(removePickableItem);
    }

    private void addPickableItem(ItemCollideWithPlayerEvent e)
    {
        collidingItems.Add(e.item);
        pickUpMessagePanel.SetActive(true);
    }

    private void removePickableItem(ItemEndCollideWithPlayerEvent e)
    {
        collidingItems.Remove(e.item);
        if (collidingItems.Count == 0)
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
                slot.GetComponent<gameObjectSlot>().gameObjectInSlot = (e.item as MonoBehaviour).gameObject;
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
