using Newtonsoft.Json.Linq;
using SDD.Events;
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

    private CharacterController characterController;
    private bool isPressingJump;
    private bool isGrounded;
    private float verticalVelocity;
    private float xInput;
    private float yInput;
    private Vector3 inputVector;
    private float currentVelocity = 0f;
    private float accelerationRate;
    private float groundCheckDistance;

    private void Awake()
    {
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
        UpdateLookMode();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        MovePlayer();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        isPressingJump = Input.GetButton("Jump");
    }

    /*
		Move the player each fixedTimeDelta
		this function is meant to be called in fixedUpdate
    */
    private void MovePlayer()
    {
        inputVector = new Vector3(xInput, 0, yInput).normalized;
        float targetVelocity = inputVector.magnitude * maxVelocity;
        if (Mathf.Abs(targetVelocity) < Mathf.Abs(currentVelocity))
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, deceleration);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, acceleration);
        }
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
        if (isPressingJump && isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }
        Vector3 movement = currentVelocity * inputVector;

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
        if (isGrounded) // if he is grounded, don't make the player fall
        {
            verticalVelocity = 0;
            return;
        }
        // no need to call TimeDelta because the function is called inside fixedUpdate
        verticalVelocity += (Physics.gravity.y / 1000f);
    }

    private void UpdateLookMode()
    {
        if (Input.GetButtonDown("Look Downwards"))
        {
            if (PlayerManager.Instance.ActivePlayerLook == PlayerLook.DOWNWARDS)
            {
                EventManager.Instance.Raise(new PlayerSwitchLookModeEvent { lookMode = PlayerLook.AHEAD });

            }
            else
            {
                EventManager.Instance.Raise(new PlayerSwitchLookModeEvent { lookMode = PlayerLook.DOWNWARDS });

            }
            return;
        }
        if (Input.GetButtonDown("Look Upwards"))
        {
            if (PlayerManager.Instance.ActivePlayerLook == PlayerLook.UPWARDS)
            {
                EventManager.Instance.Raise(new PlayerSwitchLookModeEvent { lookMode = PlayerLook.AHEAD });
            }
            else
            {
                EventManager.Instance.Raise(new PlayerSwitchLookModeEvent { lookMode = PlayerLook.UPWARDS });
            }
        }
    }

    private void PerformGroundCheck()
    { 
        groundCheckDistance = (characterController.height / 2) + 0.1f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
