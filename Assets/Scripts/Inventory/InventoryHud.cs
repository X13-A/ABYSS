using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : MonoBehaviour, IEventHandler
{
    [SerializeField] private GameObject pickUpMessagePanel;
    [SerializeField] private List<GameObject> slots;

    private HashSet<IInventoryItem> collidingItems = new HashSet<IInventoryItem>();

    private void OnEnable()
    {
        LoadHud();

        SubscribeEvents();
    }

    private void Start()
    {
        LoadHud();
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

    private void SetSlotContent(int slot, int count, IInventoryItem item)
    {
        Transform slotTransform = this.slots[slot].transform;
        Transform imageTransform = slotTransform.GetChild(0).GetChild(0);
        Transform countItem = imageTransform.GetChild(0);
        TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();

        Image image = imageTransform.GetComponent<Image>();
        ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

        // Set count
        textCountItem.text = count.ToString();

        // Set drag item
        itemDragHandler.Item = item;

        GameObject itemGameObject = (item as MonoBehaviour).gameObject;

        // Set gameObject in slot
        slotTransform.GetComponent<gameObjectSlot>().gameObjectInSlot = itemGameObject;

        // Set slot image
        image.sprite = item.Image;

        // Enable image and text
        image.enabled = true;
        textCountItem.enabled = true;
    }

    private void LoadHud()
    {
        foreach (KeyValuePair<string, GameObject> kvp in PlayerData.Instance.Items)
        {
            Debug.Log(kvp.Value);
            IInventoryItem item = kvp.Value.GetComponent<IInventoryItem>();
            int count = -1;
            int slotIndex = -1;

            string name = kvp.Key;
            PlayerData.Instance.ItemsCount.TryGetValue(name, out count);
            PlayerData.Instance.ItemsSlot.TryGetValue(name, out slotIndex);

            if (count == -1 || slotIndex == -1 || item == null) return;
            SetSlotContent(slotIndex, count, item);
        }
    }

    private void InventoryScript_ItemAdded(ItemAddedEvent e)
    {
        LoadHud();
    }

    private void InventoryScript_ItemRemoved(ItemRemovedEvent e)
    {
        LoadHud();
    }
}
