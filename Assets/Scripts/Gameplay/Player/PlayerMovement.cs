using Newtonsoft.Json.Linq;
using SDD.Events;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    /**
     * <summary>
     * This class main purpose is the player movement
     * this include horizontal and vertical movement, and jump.
     * </summary>
     **/
    [Header("Movement")]
    [SerializeField] private float maxVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float groundDrag;

    private CharacterController characterController;
    private bool isPressingJump;
    private bool isGrounded;
    private float verticalVelocity;
    private float xInput;
    private float yInput;
    private Vector3 inputVector;
    private float currentVelocity = 0f;
    private float groundCheckDistance;
    private float targetVelocity;
    private Collider playerCollider;
    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // all of the code inside this function will be executed each frame
        // the physics should be puted inside fixedUpdate
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            xInput = 0;
            yInput = 0;
            isPressingJump = false;
            return;
        }
        PerformGroundCheck();
        HandleInput();
        UpdateTargetVelocity();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        ApplyGravity();
        MovePlayer();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        isPressingJump = Input.GetButton("Jump");
    }

    private void UpdateTargetVelocity()
    {
        inputVector = new Vector3(xInput, 0, yInput);
        if (inputVector.magnitude > 1)
        {
            inputVector = inputVector.normalized;
        }
        targetVelocity = inputVector.magnitude * maxVelocity;
    }

    /*
		Move the player each fixedTimeDelta
		this function is meant to be called in fixedUpdate
    */
    private void MovePlayer()
    {
        if (Mathf.Abs(currentVelocity) < Mathf.Abs(targetVelocity))
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, acceleration);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, deceleration);
        }
        if (isGrounded) // apply ground drag
        {
            currentVelocity *= (1 - groundDrag);
        }
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
        if (isPressingJump && isGrounded)
        {
            //EventManager.Instance.Raise(new MessageEvent
            //{
            //     delay = 0.05f,
            //     text = LoremIpsum.Generate(30)
            //});
            verticalVelocity = jumpSpeed;
        }
        Vector3 movement = inputVector * currentVelocity;
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        movement.y = verticalVelocity;
        movement = transform.TransformDirection(movement);
        characterController.Move(movement);
    }

    /*
	    Apply the gravity to the player verticalVelocity 
	    meant to be called inside fixedUpdate
    */
    private void ApplyGravity()
    {
        // no need to call TimeDelta because the function is called inside fixedUpdate
        verticalVelocity += (Physics.gravity.y / 1000f);
    }

    private void PerformGroundCheck()
    {

        groundCheckDistance = 0.05f;

        Vector3 top = characterController.bounds.center + Vector3.up * (characterController.height / 2 - characterController.radius);
        Vector3 bottom = characterController.bounds.center + Vector3.down * (characterController.height / 2 - characterController.radius) - new Vector3(0, groundCheckDistance, 0);
        float radius = characterController.radius;

        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        isGrounded = Physics.CheckCapsule(top, bottom, radius, layerMask);

        // Color of the capsule in the scene view.
        Color color = isGrounded ? Color.green : Color.red;

        // Draw the top and bottom caps of the capsule.
        for (int i = 0; i < 360; i++)
        {
            float angle = i * Mathf.PI / 180f;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Debug.DrawLine(top + offset, top + Quaternion.Euler(90, 0, 0) * offset, color);
            Debug.DrawLine(bottom + offset, bottom + Quaternion.Euler(90, 0, 0) * offset, color);
        }

        // Draw the lines connecting the caps.
        Debug.DrawLine(top + Vector3.left * radius, bottom + Vector3.left * radius, color);
        Debug.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius, color);
        Debug.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius, color);
        Debug.DrawLine(top + Vector3.back * radius, bottom + Vector3.back * radius, color);
        Debug.Log("TEST");


        // Debugging purpose
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 1000, Color.green);
        }
    }
}
