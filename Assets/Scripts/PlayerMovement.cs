using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;
    Vector3 vel;
    Vector3 rot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        vel = new Vector3(xInput, 0, yInput).normalized;
    }

    private void FixedUpdate()
    {

        //rb.AddForce(vel , ForceMode.Force);
        rb.velocity = new Vector3(vel.x * moveSpeed, 0, vel.z * moveSpeed);
    }
}
