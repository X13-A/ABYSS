using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.UI;

public class ImageController : MonoBehaviour, IEventHandler
{
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
        EventManager.Instance.AddListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);

    }
    public void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        GetComponent<RawImage>().texture = e.updatedRt;
    }
}
