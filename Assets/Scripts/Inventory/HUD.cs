using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Inventory Inventory;

    public GameObject MessagePanel;

    // Start is called before the first frame update
    void Start()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += InventoryScript_ItemRemoved;
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform countItem = imageTransform.GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();
            if (image.enabled && image.sprite == e.Item.Image)
            {
                textCountItem.text = e.Count.ToString();
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
                image.sprite = e.Item.Image;
                itemDragHandler.Item = e.Item;
                textCountItem.text = e.Count.ToString();
                break;
            }
        }
    }

    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform countItem = imageTransform.GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            TextMeshProUGUI textCountItem = countItem.GetComponent<TextMeshProUGUI>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (itemDragHandler.Item.Equals(e.Item))
            {
                image.enabled = false;
                image.sprite = null;
                textCountItem.enabled = false;
                //itemDragHandler.Item = null;

                break;
            }
        }
    }

    public void OpenMessagePanel(string text)
    {
        MessagePanel.SetActive(true);
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
    }
}
