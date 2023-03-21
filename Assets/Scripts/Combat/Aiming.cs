using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Aiming : MonoBehaviour, IEventHandler
{
    private static Aiming m_Instance;
    public static Aiming Instance { get { return m_Instance; } }

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject target;
    [SerializeField] private RenderTexture rt;
    [SerializeField] private GameObject rotatingBody;
    [SerializeField] private AimingMode aimingMode;
    public AimingMode AimingMode { get { return aimingMode; } }

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
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(ToggleAim);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(ToggleAim);
    }
    private void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        this.rt = e.updatedRt;
    }
    private void ToggleAim(AimingModeUpdateEvent e)
    {
        // Reset rotation of model to face camera
        this.rotatingBody.transform.localRotation = Quaternion.identity;
        this.aimingMode = e.mode;

        // Lock or free cursor
        if (e.mode == AimingMode.CAMERA) Cursor.lockState = CursorLockMode.Locked;
        if (e.mode == AimingMode.CURSOR) Cursor.lockState = CursorLockMode.None;
    }

    private void Aim()
    {
        // Convert screen position to camera position
        Vector3 mousePos = Input.mousePosition;
        float xRatio = (float) this.rt.width / Screen.width;
        float yRatio = (float) this.rt.height / Screen.height;
        mousePos.x *= xRatio;
        mousePos.y *= yRatio;

        // Cast ray from camera to find where to aim
        Ray ray = this.cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Aim")))
        {
            this.target.transform.position = hit.point;
            this.rotatingBody.transform.LookAt(new Vector3(this.target.transform.position.x, this.transform.position.y, this.target.transform.position.z));
        }
    }

    private void Controls()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            EventManager.Instance.Raise(new AimingModeUpdateEvent { mode = AimingMode.CURSOR });
        }
        if (Input.GetButtonUp("Fire2"))
        {
            EventManager.Instance.Raise(new AimingModeUpdateEvent { mode = AimingMode.CAMERA });
        }
    }
    private bool IsAiming()
    {
        return GameManager.Instance.State == GAMESTATE.PLAY && this.AimingMode == AimingMode.CURSOR;
    }

    private bool IsActive()
    {
        return (GameManager.Instance.State == GAMESTATE.PLAY);
    }

    private void Update()
    {
        if (this.IsActive())
        {
            this.Controls();
        }
        if (this.IsAiming())
        {
            this.Aim();
        }
    }
}
