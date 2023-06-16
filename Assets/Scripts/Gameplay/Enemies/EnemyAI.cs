using SDD.Events;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float runningVelocity;
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float runningWhenModifier;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackVariantDuration;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackVariantDamage;
    [SerializeField] private int attackVariantFrequency;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float jumpForce;

    // the distance the enemy will keep with the player when attacking
    [SerializeField] private float attackDistanceOffset;
    public float detectionRadius;

    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody rb;
    private Collider enemyCollider;

    private float distanceToPlayer;
    private bool isWalking;
    private bool isRunning;
    private float attackStartTime;
    private int attackVariant;
    private int attackCounter;
    private bool isReadyToBattle;
    private bool canAttack;
    private bool isJumping;
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
    }

    private void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        attackVariant = 0;
        attackCounter = 0;
        distanceToPlayer = Mathf.Infinity;
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
        if (distanceToPlayer > detectionRadius) return;
        if (IsFacingWall())
        {
            /// TODO ne pas gérer ça ici mais plutot set une valeur boolean
            // is facing wall pour gérer ce cas et ne plus faire bouger l'enemy
            isJumping = false;
            isWalking = false;
            isRunning = false;
            canAttack = false;
        }
        else
        {
            if (IsFacingBlock())
            {
                canAttack = false;
                isJumping = true;
            }
            else
            {
                canAttack = true;
                isJumping = false;
            }
        }


        if (distanceToPlayer < attackDistanceOffset && AttackElaspedTime > attackDuration && canAttack)
        {
            TriggerAttack();
            attackStartTime = Time.time;
        }

        else
        {
            if (isJumping)
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

    private void UpdateStatus()
    {
        if (PlayerManager.Instance.PlayerReference == null)
        {
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.PlayerReference.position);
        UpdateRunningAndWalkingStatus();
    }

    private void UpdateRunningAndWalkingStatus()
    {
        isRunning = false;
        isWalking = false;
        isReadyToBattle = false;

        if (distanceToPlayer >= detectionRadius)
        {
            return;
        }

        if (distanceToPlayer <= attackDistanceOffset && canAttack)
        {
            isReadyToBattle = true;
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

    /// <summary>
    /// whill perform check for wall in front of the gameObject
    /// </summary>
    private bool IsFacingBlock()
    {
        bool hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), GetEnemyHalfWidth().x + 1f);
        return hit;
    }

    private bool IsFacingWall()
    {
        // perform the raycast one block taller,
        // if the raycast hit something, we are facing a wall with two block or more
        bool hit = Physics.Raycast
        (
            transform.position + new Vector3(0, 1, 0),
            transform.TransformDirection(Vector3.forward),
            GetEnemyHalfWidth().x + 1f
        );
        return hit;
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
        Vector3 playerWidth = new Vector3(PlayerManager.Instance.PlayerReference.GetComponent<CharacterController>().radius, 0, 0);

        Vector3 enemyWidth = GetEnemyHalfWidth();
        Vector3 directionToPlayer = ((PlayerManager.Instance.PlayerReference.position + playerWidth) - (transform.position + enemyWidth)).normalized;
        directionToPlayer.y = 0; // remove any influence from the y axis
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        rb.MovePosition(transform.position + directionToPlayer * Velocity);
    }

    private Vector3 GetEnemyHalfWidth()
    {
        if (enemyCollider != null)
        {
            if (enemyCollider is CapsuleCollider capsule)
            {
                return new Vector3(capsule.radius, 0, 0);
            }
            if (enemyCollider is BoxCollider box)
            {
                return new Vector3(box.size.x / 2, 0, 0);
            }
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


    // Triggers the attack animation
    private void TriggerAttack()
    {
        attackCounter++;
        attackVariant = attackCounter % attackVariantFrequency == 0 ? 1 : 0;
        animationController.TriggerAttack();
        audioSource.PlayOneShot(attackSound);
    }


    // Is triggered when the attack animation starts (using animation events)
    private void StartAttack(int variant)
    {
        enemyAttack.StartDamage(variant);
    }

    // Is triggered when the attack animation ends
    private void StopAttack()
    {
        enemyAttack.EndDamage();
    }
}
