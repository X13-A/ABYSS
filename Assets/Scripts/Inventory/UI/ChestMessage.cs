using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestMessage : MonoBehaviour
{
    [SerializeField] GameObject messagePanel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string openKey = "E";

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
        EventManager.Instance.AddListener<UpdateCollidingChestsEvent>(this.ToggleDisplay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateCollidingChestsEvent>(this.ToggleDisplay);
    }

    private void ToggleDisplay(UpdateCollidingChestsEvent e)
    {
        if (e.chests.Count == 0)
        {
            this.messagePanel.SetActive(false);
        }
        else
        {
            this.messagePanel.SetActive(true);
            this.text.text = $"Press {this.openKey} to open chest";
        }
    }
}
