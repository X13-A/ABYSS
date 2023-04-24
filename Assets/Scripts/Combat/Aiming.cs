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
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(RotatePlayer);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<RenderTextureUpdateEvent>(UpdateRenderTexture);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(RotatePlayer);
    }

    private void UpdateRenderTexture(RenderTextureUpdateEvent e)
    {
        rt = e.updatedRt;
    }

    private void RotatePlayer(AimingModeUpdateEvent e)
    {
        // Reset rotation of model to face camera
        if (e.mode == AimingMode.CAMERA)
        {
            StartCoroutine(CoroutineUtil.RotateTo(rotatingBody.transform, 0.1f, Quaternion.identity));
        }
    }

    private void Aim()
    {
        RaycastHit hit = AimUtil.Instance.Aim();
        if (hit.collider)
        {
            target.transform.position = hit.point;
            rotatingBody.transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
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
        if (PlayerManager.Instance == null) return false;
        return GameManager.Instance.State == GAMESTATE.PLAY && PlayerManager.Instance.ActiveAimingMode == AimingMode.CURSOR;
    }

    private bool IsActive()
    {
        return (GameManager.Instance.State == GAMESTATE.PLAY);
    }

    private void Update()
    {
        if (IsActive())
        {
            Controls();
        }
        if (IsAiming())
        {
            Aim();
        }
    }
}
