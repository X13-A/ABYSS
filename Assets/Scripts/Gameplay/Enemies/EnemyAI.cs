using SDD.Events;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float runningVelocity;
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float runningOffsetFactor;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackVariantDuration;
    [SerializeField] private int attackVariantFrequency;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float jumpForce;
    [SerializeField] private float attackDistanceOffset;

    public float detectionRadius;

    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody rb;
    private Collider enemyCollider;

    private float distanceToPlayer;
    private float attackStartTime;
    private int attackVariant;
    private int attackCounter;
    private StatusManager status;

    private EnemyAnimationController animationController;
    private EnemyAttack enemyAttack;

    public float Velocity { get; private set; }
    public float AttackElaspedTime => Time.time - attackStartTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        animationController = GetComponent<EnemyAnimationController>();
        enemyAttack = GetComponent<EnemyAttack>();
        status = new StatusManager();
    }

    private void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        attackVariant = 0;
        attackCounter = 0;
        distanceToPlayer = Mathf.Infinity;
        status.ClearStatus();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        UpdateCurrentVelocity();
        UpdateStatus();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;

        bool canAttack = status.HasStatus(EnemyStatus.CanAttack);
        bool jumping = status.HasStatus(EnemyStatus.Jumping);


        // attack
        if (AttackElaspedTime > attackDuration && canAttack)
        {
            TriggerAttack();
            attackStartTime = Time.time;
        }
        // move toward player or jump
        else
        {
            if (jumping)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                MoveAndRotateTowardPlayer();
            }
        }
    }

    private bool IsSensing()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string layer = "FindPlayerAndWalk";
        return (stateInfo.IsName(layer + "." + "SenseSomethingStart") || stateInfo.IsName(layer + "." + "SenseSomethingMain")) && stateInfo.normalizedTime < 1;
    }

    /// <summary>
    /// will check for all parameters that lead to a change of status
    /// each status is cleared and reset in this function
    /// </summary>
    private void UpdateStatus()
    {
        if (PlayerManager.Instance.PlayerReference == null)
        {
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.PlayerReference.position);

        status.ClearStatus();


        if (distanceToPlayer >= detectionRadius)
        {
            return;
        }


        if (IsFacingBlock())
        {
            if (IsFacingWall())
            {
                return;
            }
            status.SetStatus(EnemyStatus.Jumping);
            return;
        }

        if (distanceToPlayer <= attackDistanceOffset)
        {
            status.SetStatus(EnemyStatus.ReadyToBattle);
            status.SetStatus(EnemyStatus.CanAttack);
            return;
        }

        if (distanceToPlayer > detectionRadius / runningOffsetFactor)
        {
            status.SetStatus(EnemyStatus.Walking);
        }
        else
        {
            status.SetStatus(EnemyStatus.Running);
        }
    }

    /// <summary>
    /// whill perform check for wall in front of the gameObject
    /// </summary>
    private bool IsFacingBlock()
    {
        return RaycastThrow(Vector3.zero, GetEnemyHalfWidth() + 0.2f);
    }

    private bool IsFacingWall()
    {
        // perform the raycast one block taller,
        // if the raycast hit something, we are facing a wall with two block or more
        return RaycastThrow(new Vector3(0, 1, 0), GetEnemyHalfWidth() + 0.2f);
    }

    private bool RaycastThrow(Vector3 offset, float distance)
    {

        Vector3 pos = transform.position + offset;
        RaycastHit hit;
        bool result;
        result = Physics.Raycast(pos, transform.forward, out hit, distance, 1 << LayerMask.NameToLayer("Ground"));
        // to debug the raycast in the scene
        if (result)
        {
            Debug.DrawRay(pos, transform.forward * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(pos, transform.forward * 10, Color.green);
        }
        return result;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalking", status.HasStatus(EnemyStatus.Walking));
        animator.SetBool("isRunning", status.HasStatus(EnemyStatus.Running));
        animator.SetBool("isReadyToBattle", status.HasStatus(EnemyStatus.ReadyToBattle));
        animator.SetInteger("attackVariant", attackVariant);
    }

    private void MoveAndRotateTowardPlayer()
    {
        if (IsSensing()) // do not move if the enemy is sensing something
        {
            return;
        }
        Vector3 playerWidth = new Vector3(PlayerManager.Instance.PlayerReference.GetComponent<CharacterController>().radius, 0, 0);

        float enemyWidth = GetEnemyHalfWidth();
        Vector3 directionToPlayer = ((PlayerManager.Instance.PlayerReference.position + playerWidth) - (transform.position + new Vector3(enemyWidth, 0, 0))).normalized;
        directionToPlayer.y = 0; // remove any influence from the y axis
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        rb.MovePosition(transform.position + directionToPlayer * Velocity);
    }

    private float GetEnemyHalfWidth()
    {
        if (enemyCollider == null)
        {
            return 0f;
        }
        if (enemyCollider is CapsuleCollider capsule)
        {
            return capsule.radius;
        }
        if (enemyCollider is BoxCollider box)
        {
            return box.size.x / 2;
        }
        return 0f;
    }

    private void UpdateCurrentVelocity()
    {
        bool walking = status.HasStatus(EnemyStatus.Walking);
        bool running = status.HasStatus(EnemyStatus.Running);

        if (walking && !running)
        {
            Velocity = walkingVelocity;
        }
        else if (running && !walking)
        {
            Velocity = runningVelocity;
        }
        else
        {
            Velocity = 0f;
        }
    }

    // Triggers the attack animation
    private void TriggerAttack()
    {
        attackCounter++;
        attackVariant = attackCounter % attackVariantFrequency == 0 ? 1 : 0;
        animationController.TriggerAttack();
    }

    // Is triggered when the attack animation starts (using animation events)
    private void StartAttack(int variant)
    {
        enemyAttack.StartDamage(variant);
        audioSource.PlayOneShot(attackSound);
    }

    // Is triggered when the attack animation ends
    private void StopAttack()
    {
        enemyAttack.EndDamage();
    }
}
