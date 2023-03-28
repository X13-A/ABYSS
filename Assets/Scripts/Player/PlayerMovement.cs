using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /**
     * <summary>
     * This class main purpoose is the player movement
     * this include horizontal and vertical movement, and jump.
     * </summary>
     **/
    [Header("Movement")]
    [SerializeField] private float maxVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float gravity;

    private CharacterController characterController;
    private bool isJumping;
    private bool isGrounded;
    private float verticalVelocity;
    private float xInput;
    private float yInput;
    private Vector3 inputVector;
    private float currentVelocity = 0f;
    private float accelerationRate;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void MovePlayer()
    {
        inputVector = new Vector3(xInput, 0, yInput).normalized;
        float targetVelocity = inputVector.magnitude * maxVelocity;
        if (Mathf.Abs(targetVelocity) < Mathf.Abs(currentVelocity))
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
        Vector3 movement = currentVelocity * Time.fixedDeltaTime * inputVector;
        if (isJumping && isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }
        Debug.Log(currentVelocity);
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        movement = transform.TransformDirection(movement);
        movement.y = verticalVelocity;
        characterController.Move(movement);
    }
    private void HandleInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        isJumping = Input.GetButton("Jump");
    }

    private void UpdateGravity()
    {
        if (isGrounded)
        {
            verticalVelocity = 0;
            return;
        }
        verticalVelocity += Physics.gravity.y * Time.deltaTime / 6f;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.1f);
        HandleInput();
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        MovePlayer();
    }
}
