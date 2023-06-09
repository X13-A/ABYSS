using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignMessage : MonoBehaviour
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
        EventManager.Instance.AddListener<ShowSignInteractionMessage>(ShowDisplay);
        EventManager.Instance.AddListener<HideSignInteractionMessage>(HideDisplay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ShowSignInteractionMessage>(ShowDisplay);
        EventManager.Instance.RemoveListener<HideSignInteractionMessage>(HideDisplay);
    }

    private void ShowDisplay(ShowSignInteractionMessage e)
    {
        messagePanel.SetActive(true);
        text.text = $"Press {openKey} to read the sign";
    }

    private void HideDisplay(HideSignInteractionMessage e)
    {
        messagePanel.SetActive(false);
    }
}
