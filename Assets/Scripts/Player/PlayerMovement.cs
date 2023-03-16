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
    [SerializeField] private float gravity;

    private CharacterController characterController;
    private Vector3 direction;
    private bool isJumping;
    private float verticalVelocity;

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
        characterController = GetComponent<CharacterController>();
        characterController.center = new Vector3(0, 0.5f, 0); // relative height (so 0.5 is the center of our gameObject)
        characterController.height = transform.lossyScale.y;
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(direction.x * velocity, 0, direction.z * velocity);
        // the player may have rotated in his own referential, so,
        // we need to convert his local position and rotation to a global one
        movement = transform.TransformDirection(movement);
        movement.y = verticalVelocity;
        characterController.Move(movement);
    }
    private void HandleInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        isJumping = Input.GetButtonDown("Jump") && characterController.isGrounded;
        direction = new Vector3(xInput, 0, yInput).normalized;
    }

    private void UpdateGravity()
    {
        if (characterController.isGrounded)
        {

            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity -= gravity;
        }
    }

    private float GetDrag()
    {
        if (characterController.isGrounded)
        {
            return 5f;
        }
        return 0f;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }
        HandleInput();
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        MovePlayer();
    }
}
