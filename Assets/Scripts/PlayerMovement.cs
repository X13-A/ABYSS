using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocity;
    Rigidbody rb;
    private Vector3 movement;
 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");
        movement = transform.forward * zInput + transform.right * xInput;
    }

    void FixedUpdate()
    {
        rb.AddForce(movement * velocity, ForceMode.Force);
    }
}
