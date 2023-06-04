using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private ItemId? itemId = null;
    public ItemId? ItemId => itemId;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image selectedFrame;

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            selectedFrame.enabled = true;
        }
        else
        {
            selectedFrame.enabled = false;
        }
    }

    public void UpdateInfo(InventoryItem item)
    {
        if (item == null)
        {
            this.itemId = null;
            this.image.sprite = null;
            this.image.enabled = false;
            this.text.text = string.Empty;
            this.text.enabled = false;
        }
        else
        {
            this.itemId = item.Id;
            this.image.sprite = ItemBank.GetIcon(item.Id);
            this.image.enabled = true;
            this.text.text = item.Count.ToString();
            this.text.enabled = true;
        }
    }
}
