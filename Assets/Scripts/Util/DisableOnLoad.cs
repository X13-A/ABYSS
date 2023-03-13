using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class DisableOnLoad : MonoBehaviour, IEventHandler
{
    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<SceneReadyToChangeEvent>(Disable);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SceneReadyToChangeEvent>(Disable);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Disable(SceneReadyToChangeEvent e)
    {
        gameObject.SetActive(false);
    }
}
