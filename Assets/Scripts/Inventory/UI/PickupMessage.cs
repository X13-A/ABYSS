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
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<UpdateCollidingItemsEvent>(ToggleDisplay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateCollidingItemsEvent>(ToggleDisplay);
    }

    private void ToggleDisplay(UpdateCollidingItemsEvent e)
    {
        if (e.items.Count == 0)
        {
            messagePanel.SetActive(false);
        }
        else
        {
            messagePanel.SetActive(true);
            text.text = $"Press {pickupKey} to pickup ({e.items.Count})";
        }
    }
}
