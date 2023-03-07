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
        movement = Vector3.zero;
        if (GameManager.Instance.State != GAMESTATE.play) return;

        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        movement = new Vector3(xInput, 0, yInput).normalized;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(movement.x * velocity, 0, movement.z * velocity);
    }
}
