using SDD.Events;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float runningVelocity;
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float runningWhenModifier;
    [SerializeField] private Transform playerReference;
    [SerializeField] private float currentAttackDuration;
    public float detectionRadius;

    private Animator animator;
    private Rigidbody rb;
    private CharacterController playerCharacterController;
    private CapsuleCollider enemyCollider;

    private float distanceToPlayer;
    private bool isWalking;
    private bool isRunning;
    private float attackOffset;
    private float attackStartTime;
    private int attackVariant;
    private int attackCounter;

    public float Velocity { get; private set; }
    public float AttackElaspedTime => Time.time - attackStartTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCharacterController = playerReference.GetComponent<CharacterController>();
        enemyCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        attackOffset = 1f;
        attackStartTime = Time.time - 1000;
        attackVariant = 0;
        attackCounter = 0;
    }

    private void Update()
    {
        UpdateCurrentVelocity();
        UpdateStatus();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (distanceToPlayer <= detectionRadius)
        {
            if (distanceToPlayer > attackOffset)
            {
                MoveAndRotateTowardPlayer();
            }
            else if (AttackElaspedTime > currentAttackDuration)
            {
                RaiseAttackEvent();
                attackStartTime = Time.time;
            }
        }
    }

    private void UpdateStatus()
    {
        if (playerReference == null)
        {
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, playerReference.position);
        UpdateRunningAndWalkingStatus();
    }

    private void UpdateRunningAndWalkingStatus()
    {
        if (distanceToPlayer <= attackOffset)
        {
            isRunning = false;
            isWalking = false;
        }
        else if (distanceToPlayer >= detectionRadius)
        {
            isWalking = false;
            isRunning = false;
        }
        else if (distanceToPlayer <= detectionRadius && distanceToPlayer > detectionRadius / runningWhenModifier)
        {
            isWalking = true;
            isRunning = false;
        }
        else if (distanceToPlayer > 0 && distanceToPlayer <= detectionRadius / runningWhenModifier)
        {
            isRunning = true;
            isWalking = false;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetInteger("attackVariant", attackVariant);
    }

    private void MoveAndRotateTowardPlayer()
    {
        Vector3 player_width = new Vector3(playerCharacterController.radius, 0, 0);
        Vector3 enemy_width = new Vector3(enemyCollider.radius, 0, 0);
        Vector3 directionToPlayer = ((playerReference.position + player_width) - (transform.position + enemy_width)).normalized;
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        rb.MovePosition(transform.position + directionToPlayer * Velocity);
    }

    private void UpdateCurrentVelocity()
    {
        if (isWalking && !isRunning)
        {
            Velocity = walkingVelocity;
        }
        else if (isRunning && !isWalking)
        {
            Velocity = runningVelocity;
        }
        else
        {
            Velocity = 0f;
        }
    }

    private void RaiseAttackEvent()
    {
        EventManager.Instance.Raise(new EnemyAttackEvent
        {
            damage = 10f
        });
        attackCounter++;
        attackVariant = attackCounter % 3 == 0 ? 1 : 0;
    }
}
