using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimUtil : MonoBehaviour, IEventHandler
{
    private static AimUtil m_Instance;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject target;
    [SerializeField] private RenderTexture rt;

    public static AimUtil Instance { get { return m_Instance; } }

    private void Awake()
    {
        if (!m_Instance) m_Instance = this;
        else Destroy(gameObject);
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
        EventManager.Instance.AddListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public RaycastHit Aim(int layerMask)
    {
        // Convert screen position to camera position
        Vector3 mousePos = Input.mousePosition;
        float xRatio = (float) rt.width / Screen.width;
        float yRatio = (float) rt.height / Screen.height;
        mousePos.x *= xRatio;
        mousePos.y *= yRatio;

        // Cast ray from camera to find where to aim
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        return hit;
    }

    private void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        rt = e.updatedRt;
    }
}
