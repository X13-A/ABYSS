using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private static MouseLock m_Instance;
    public static MouseLock Instance => m_Instance;

    [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsActive()
    {
        if (GameManager.Instance.State == GAMESTATE.PLAY)
        {
            return Aiming.Instance.AimingMode == AimingMode.CAMERA;
        }
        else
        {
            return false;
        }
    }

    private void FixedUpdate()
    {

        if (IsActive())
        {
            float cameraRotationY = freeLookCamera.m_XAxis.Value;
            characterController.transform.rotation = Quaternion.Euler(0f, cameraRotationY, 0f);
        }
    }
}
