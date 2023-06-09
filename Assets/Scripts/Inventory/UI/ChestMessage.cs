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
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<UpdateCollidingChestsEvent>(ToggleDisplay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateCollidingChestsEvent>(ToggleDisplay);
    }

    private void ToggleDisplay(UpdateCollidingChestsEvent e)
    {
        if (e.chests.Count == 0)
        {
            messagePanel.SetActive(false);
        }
        else
        {
            messagePanel.SetActive(true);
            text.text = $"Press {openKey} to open chest";
        }
    }
}
