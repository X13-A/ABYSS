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
            itemId = null;
            image.sprite = null;
            image.enabled = false;
            text.text = string.Empty;
            text.enabled = false;
        }
        else
        {
            itemId = item.Id;
            image.sprite = ItemBank.GetIcon(item.Id);
            image.enabled = true;
            text.text = item.Count.ToString();
            text.enabled = true;
        }
    }
}
