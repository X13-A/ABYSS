using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupMessage : MonoBehaviour
{
    [SerializeField] GameObject messagePanel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string pickupKey = "E";

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
        EventManager.Instance.AddListener<UpdateCollidingItemsEvent>(this.ToggleDisplay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateCollidingItemsEvent>(this.ToggleDisplay);
    }

    private void ToggleDisplay(UpdateCollidingItemsEvent e)
    {
        if (e.items.Count == 0)
        {
            this.messagePanel.SetActive(false);
        }
        else
        {
            this.messagePanel.SetActive(true);
            this.text.text = $"Press {pickupKey} to pickup ({e.items.Count})";
        }
    }
}
