using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDrag;

    private Rigidbody rb;
    private Vector3 direction;
    private bool isJumping;

    [Header("Ground check")]
    [SerializeField] private float playerHeight;
    private bool isGrounded;

    private void Start()
    {
        /*
         * Alexis : Mis en commentaire parce qu'on peut plus cliquer sur les menus sinon
         */
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(direction.x * velocity, 0, direction.z * velocity);
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        movement = transform.TransformDirection(movement);
        rb.AddForce(movement * 10, ForceMode.Force);
        if (this.isJumping)
        {
            this.rb.AddForce(new Vector3(0f, this.jumpForce, 0f), ForceMode.Impulse);
            this.isJumping = false;
        }
    }

    private void HandleInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        this.isJumping = Input.GetButtonDown("Jump") && this.isGrounded;
        this.direction = new Vector3(xInput, 0, yInput).normalized;
    }

    void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.play)
            return;
        this.isGrounded = Physics.Raycast(rb.position, Vector3.down, playerHeight * 0.5f + 0.2f, LayerMask.GetMask("Ground"));
        this.HandleInput();
        if (this.isGrounded)
            this.rb.drag = this.groundDrag;
        else
        {
            this.rb.drag = 10;
            this.rb.AddForce(Physics.gravity, ForceMode.VelocityChange);
        }
    }

    private void FixedUpdate()
    {
        this.MovePlayer();
    }
}
