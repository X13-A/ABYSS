using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.Events;
using System;

public class RenderManager : MonoBehaviour, IEventHandler
{
    private static RenderManager m_Instance;
    public static RenderManager Instance => m_Instance;

    private int screenWidth;
    private int screenHeight;
    public int ScreenWidth => screenWidth;
    public int ScreenHeight => screenHeight;


    [SerializeField] private int maxHeight;
    public int MaxHeight => maxHeight;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        FireResizeEvent();
    }

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
        EventManager.Instance.AddListener<ResolutionScaleUpdateEvent>(FireResizeEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ResolutionScaleUpdateEvent>(FireResizeEvent);
    }

    public void FireResizeEvent(dynamic e = null)
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

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

    private void Update()
    {
        if (Screen.width != screenWidth || Screen.height != screenHeight)
        {
            FireResizeEvent();
        }
    }
}
