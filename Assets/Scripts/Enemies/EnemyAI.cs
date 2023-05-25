using SDD.Events;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;

    public float detectionRadius;
    [SerializeField] private float runningVelocity;
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float runningWhenModifier;
    [SerializeField] private Transform playerReference;
    [SerializeField] private float currentAttackDuration;

    private float distanceToPlayer;
    private bool isWalking;
    private bool isRunning;
    private float attackOffset;

    public float Velocity { get; private set; }
    private float attackStartTime;
    public float AttackElaspedTime => Time.time - attackStartTime;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
	{
        attackOffset = 1f;        
        attackStartTime = Time.time - 1000;
	}

    private void Update()
    {
        UpdateCurrentVelocity();
        SearchPlayer();
        UpdateAnimator();
    }

    private void SearchPlayer()
    {
        if (playerReference == null)
        {
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, playerReference.position);

        if (distanceToPlayer >= detectionRadius) // OOD
        {
            isWalking = false;
            isRunning = false;
	    }
        if (distanceToPlayer <= detectionRadius && distanceToPlayer > detectionRadius / runningWhenModifier) // walking
	    {
            isWalking = true;
            isRunning = false;
	    } else if (distanceToPlayer > 0 && distanceToPlayer <= detectionRadius / runningWhenModifier) // running
	    {
            isRunning = true;
            isWalking = false;
	    }
    }

    private void UpdateAnimator()
    {
        m_Animator.SetFloat("distanceToPlayer", distanceToPlayer);
        m_Animator.SetBool("isWalking", isWalking);
        m_Animator.SetBool("isRunning", isRunning);
    }

    private void FixedUpdate()
    {
        if (distanceToPlayer <= detectionRadius)
        {
            if (distanceToPlayer > attackOffset)
            { 
			    MoveTowardPlayer();
	        } 
	        else if (AttackElaspedTime > currentAttackDuration)
	        {
			    EventManager.Instance.Raise(new CactusAttackEvent
			    {
			    	damage = 10f
			    });
                attackStartTime = Time.time;
	        }
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 player_width = new Vector3(playerReference.GetComponent<CharacterController>().radius, 0, 0);
        Vector3 enemy_width = new Vector3(GetComponent<CapsuleCollider>().radius, 0, 0);
	    Vector3 directionToPlayer = ((playerReference.position + player_width) - (transform.position + enemy_width)).normalized;
	    m_Rigidbody.MoveRotation(Quaternion.LookRotation(directionToPlayer));
	    m_Rigidbody.MovePosition(transform.position + directionToPlayer * Velocity);
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
}
