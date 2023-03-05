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
        if (GameManager.Instance.State != GAMESTATE.play) return;
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        vel = new Vector3(xInput, 0, yInput).normalized * moveSpeed;
        rb.transform.position = new Vector3(transform.position.x - vel.x * Time.deltaTime, transform.position.y, transform.position.z - vel.z * Time.deltaTime);
    }
}
