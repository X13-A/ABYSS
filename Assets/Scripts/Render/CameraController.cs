using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class CameraController : MonoBehaviour, IEventHandler
{
    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }
    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<WindowResizeEvent>(Resize);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<WindowResizeEvent>(Resize);
    }
    void Resize(WindowResizeEvent e)
    {
        GetComponent<Camera>().aspect = (float)(e.newWidth) / e.newHeight;
    }
}



