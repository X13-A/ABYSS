using SDD.Events;
using Unity.VisualScripting;
using UnityEngine;

public class BossAI : MonoBehaviour, IEventHandler
{
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackDamage;

    // Charge attack params

    // Magic attack params

    [SerializeField] private AudioClip attackSound;
    // the distance the enemy will keep with the player when attacking
    [SerializeField] private float attackDistanceOffset;

    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody rb;
    private CharacterController playerCharacterController;
    private Collider enemyCollider;

    private float distanceToPlayer;

    private bool isWalking;
    private bool isAwake = false;

    private float attackStartTime;
    private bool isReadyToBattle;

    public float Velocity { get; private set; }
    public float AttackElaspedTime => Time.time - attackStartTime;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EndBossScreamerEvent>(StartAI);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EndBossScreamerEvent>(StartAI);
    }

    private void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        distanceToPlayer = Mathf.Infinity;
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void StartAI(EndBossScreamerEvent e)
    {
        StartCoroutine(CoroutineUtil.DelayAction(BossManager.Instance.TimeBeforeWakingUp, () =>
        {
            isAwake = true;
        }));
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCharacterController = PlayerManager.Instance.PlayerReference.GetComponent<CharacterController>();
        enemyCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (!isAwake) return;
        UpdateCurrentVelocity();
        UpdateStatus();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;
        if (!isAwake) return;
        if (distanceToPlayer < attackDistanceOffset && AttackElaspedTime > attackDuration)
        {
            RaiseAttackEvent();
            attackStartTime = Time.time;
        }
        else
        {
            MoveAndRotateTowardPlayer();
        }
    }

    private void UpdateStatus()
    {
        if (PlayerManager.Instance.PlayerReference == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, PlayerManager.Instance.PlayerReference.position);
        UpdateRunningAndWalkingStatus();
    }

    private void UpdateRunningAndWalkingStatus()
    {
        isReadyToBattle = false;
        isWalking = false;

        if (distanceToPlayer <= attackDistanceOffset)
        {
            isReadyToBattle = true;
            return;
        }

        // Always chase player
        isWalking = true;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isReadyToBattle", isReadyToBattle);
    }

    private void MoveAndRotateTowardPlayer()
    {
        Vector3 playerWidth = new Vector3(playerCharacterController.radius, 0, 0);

        Vector3 enemyWidth = GetEnemyHalfWidth();
        Vector3 directionToPlayer = ((PlayerManager.Instance.PlayerReference.position + playerWidth) - (transform.position + enemyWidth)).normalized;
        directionToPlayer.y = 0; // remove any influence from the y axis
        Quaternion quaternionToPlayer = Quaternion.LookRotation(directionToPlayer);
        rb.MoveRotation(quaternionToPlayer);
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
        if (isWalking)
        {
            Velocity = walkingVelocity;
        }
        else
        {
            Velocity = 0f;
        }
    }

    private void RaiseAttackEvent()
    {

    }

    public void LaunchAttackSound()
    {
       audioSource.PlayOneShot(attackSound);
    }
}
