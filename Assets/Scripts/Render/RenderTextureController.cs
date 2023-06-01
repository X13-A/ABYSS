using UnityEngine;
using SDD.Events;

public class RenderTextureController : MonoBehaviour, IEventHandler
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
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
        EventManager.Instance.AddListener<WindowResizeEvent>(ScaleRenderTexture);
        EventManager.Instance.AddListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<WindowResizeEvent>(ScaleRenderTexture);
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    private void ScaleRenderTexture(WindowResizeEvent e)
    {
        if (cam == null)
        {
            return;
        }

        RenderTexture rt = cam.targetTexture;
        RenderTextureDescriptor descriptor = rt.descriptor;

        float aspectRatio = (float) Screen.width / Screen.height;

        descriptor.height = (int)(RenderManager.Instance.MaxHeight * e.resolutionScale);
        descriptor.width = Mathf.RoundToInt(descriptor.height * aspectRatio);

        //descriptor.width = (int)(e.newWidth * e.resolutionScale);
        //descriptor.height = (int)(e.newHeight * e.resolutionScale);

        // Calculate new scaled width if height goes to 1080

        RenderTextureUpdateEvent updateEvent = new RenderTextureUpdateEvent();
        updateEvent.updatedRt = new RenderTexture(descriptor);
        updateEvent.updatedRt.filterMode = FilterMode.Point;
        EventManager.Instance.Raise(updateEvent);
    }

    private void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        cam.targetTexture.Release();
        cam.targetTexture = e.updatedRt;
        Debug.Log($"{cam.targetTexture.width}, {cam.targetTexture.height}");
    }
}
