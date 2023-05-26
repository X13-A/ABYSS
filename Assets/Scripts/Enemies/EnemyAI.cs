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
    private Collider enemyCollider;

    private float distanceToPlayer;
    private bool isWalking;
    private bool isRunning;
    private float attackOffset;
    private float attackStartTime;
    private int attackVariant;
    private int attackCounter;
    private bool isReadyToBattle;

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

    private bool IsSensing()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string layer = "FindPlayerAndWalk";
        return (stateInfo.IsName(layer + "." + "SenseSomethingStart") || stateInfo.IsName(layer + "." + "SenseSomethingMain")) && stateInfo.normalizedTime < 1;
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
        isRunning = false;
        isWalking = false;
        isReadyToBattle = false;

        if (distanceToPlayer <= attackOffset)
        {
            isReadyToBattle = true;
            return;
        }

        if (distanceToPlayer >= detectionRadius)
        {
            return;
        }

        if (distanceToPlayer > detectionRadius / runningWhenModifier)
        {
            isWalking = true;
        }
        else
        {
            isRunning = true;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isReadyToBattle", isReadyToBattle);
        animator.SetInteger("attackVariant", attackVariant);
    }

    private void MoveAndRotateTowardPlayer()
    {
        if (IsSensing()) // do not move if the enemy is sensing something
        {
            return;
        }
        Vector3 playerWidth = new Vector3(playerCharacterController.radius, 0, 0);

        Vector3 enemyWidth = GetEnemyWidth();
        Vector3 directionToPlayer = ((playerReference.position + playerWidth) - (transform.position + enemyWidth)).normalized;
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        rb.MovePosition(transform.position + directionToPlayer * Velocity);
    }

    private Vector3 GetEnemyWidth()
    {
        if (enemyCollider is CapsuleCollider)
        {
            CapsuleCollider capsule = (CapsuleCollider) enemyCollider;
            return new Vector3(capsule.radius, 0, 0);
        }
        if (enemyCollider is BoxCollider)
        {
            BoxCollider box = (BoxCollider) enemyCollider;
            return new Vector3(box.size.x, 0, 0);
        }
        Debug.Log("You need to implement this method with the new collider");
        return new Vector3(0, 0, 0);
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
