using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEditor.SceneManagement;

public class RenderTextureController : MonoBehaviour, IEventHandler
{
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
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
        EventManager.Instance.AddListener<WindowResizeEvent>(ScaleRenderTexture);
        EventManager.Instance.AddListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<WindowResizeEvent>(ScaleRenderTexture);
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }
    void ScaleRenderTexture(WindowResizeEvent e)
    {
        if (cam == null) return;
        RenderTexture rt = cam.targetTexture;
        RenderTextureDescriptor descriptor = rt.descriptor;

        descriptor.width = (int)(e.newWidth * e.resolutionScale);
        descriptor.height = (int)(e.newHeight * e.resolutionScale);

        RenderTextureUpdateEvent updateEvent = new RenderTextureUpdateEvent();
        updateEvent.updatedRt = new RenderTexture(descriptor);
        updateEvent.updatedRt.filterMode = FilterMode.Point;
        EventManager.Instance.Raise(updateEvent);
    }

    void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        cam.targetTexture.Release();
        cam.targetTexture = e.updatedRt;
    }
}