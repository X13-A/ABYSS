using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /**
     * <summary>
     * This class is used for the player movement
     * </summary>
     **/

    public float velocity;
    Rigidbody rb;
    private Vector3 movement;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

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
        Vector3 _velocity = new Vector3(movement.x * velocity, 0, movement.z * velocity);
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        _velocity = transform.TransformDirection(_velocity);
        rb.AddForce(_velocity, ForceMode.Force);
    }
}
