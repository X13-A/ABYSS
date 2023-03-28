using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private static MouseLock m_Instance;
    public static MouseLock Instance => m_Instance;

    public float mouseSensitivity;

    public float clampAngle;

    private CharacterController characterController;
    private float rotY;
    private float mouseX;

    private void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
    }

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

    // Update is called once per frame
    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
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
            rotY += mouseX * mouseSensitivity;
            Quaternion globalRotation = Quaternion.Euler(0f, rotY, 0f);
            characterController.transform.rotation = globalRotation;
        }
    }
}
