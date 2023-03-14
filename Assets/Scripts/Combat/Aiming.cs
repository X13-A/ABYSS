using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Aiming : MonoBehaviour, IEventHandler
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject target;
    [SerializeField] RenderTexture rt;
    [SerializeField] GameObject player;
    [SerializeField] bool aiming;
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
        EventManager.Instance.AddListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);

    }
    public void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        rt = e.updatedRt;
    }

    void Update()
    {
        if (!this.aiming) return;

        Vector3 mousePos = Input.mousePosition;
        float xRatio = (float)rt.width / Screen.width;
        float yRatio = (float)rt.height / Screen.height;
        mousePos.x *= xRatio;
        mousePos.y *= yRatio;

        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Aim")))
        {
            this.target.transform.position = hit.point;
            Vector3 targetPos = target.transform.position;
            targetPos.y = transform.position.y;
            this.player.transform.LookAt(targetPos);
        }
    }
}
