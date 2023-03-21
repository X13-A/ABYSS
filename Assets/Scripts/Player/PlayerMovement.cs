using System;
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
    [SerializeField] private float velocity;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float gravity;

    private CharacterController characterController;
    private Vector3 direction;
    private bool isJumping;
    private bool isGrounded;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(direction.x * velocity, 0, direction.z * velocity);
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        movement = transform.TransformDirection(movement);
        if (this.isJumping && this.isGrounded)
            verticalVelocity = jumpSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement);
    }
    private void HandleInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        this.isJumping = Input.GetButton("Jump");
        direction = new Vector3(xInput, 0, yInput);
    }

    private void UpdateGravity()
    {
        if (this.isGrounded)
        {
            this.verticalVelocity = 0;
            return;
        }
        this.verticalVelocity += Physics.gravity.y * Time.deltaTime / 6f;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }
        this.isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.1f);
        HandleInput();
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        MovePlayer();
    }
}
