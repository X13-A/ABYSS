using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private bool look;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!look) return;
        if (cam == null) return;
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
