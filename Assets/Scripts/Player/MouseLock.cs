using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public float mouseSensitivity;
    public float clampAngle;

    Rigidbody rb;
    private float rotY;
    private float mouseX;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
    }

    private void FixedUpdate()
    {
        rotY += mouseX * mouseSensitivity;
        Quaternion globalRotation = Quaternion.Euler(0f, rotY, 0f);
        rb.MoveRotation(globalRotation);
    }
}
