using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.Events;

public class RenderManager : MonoBehaviour, IEventHandler
{
    private static RenderManager m_Instance;
    public static RenderManager Instance { get; }

    int screenWidth;
    int screenHeight;

    private void Awake()
    {
        if (!m_Instance) m_Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        FireResizeEvent();
    }

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
        EventManager.Instance.AddListener<ResolutionScaleUpdateEvent>(FireResizeEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ResolutionScaleUpdateEvent>(FireResizeEvent);
    }

    public void FireResizeEvent(dynamic e = null)
    {
        if (SettingsManager.Instance == null) return;
        WindowResizeEvent resizeEvent = new WindowResizeEvent
        {
            oldWidth = screenWidth,
            oldHeight = screenHeight,
            newWidth = Screen.width,
            newHeight = Screen.height,
            resolutionScale = SettingsManager.Instance.ResolutionScale
        };
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        EventManager.Instance.Raise(resizeEvent);
    }

    void Update()
    {
        if (Screen.width != screenWidth || Screen.height != screenHeight)
        {
            FireResizeEvent();
        }
    }
}
