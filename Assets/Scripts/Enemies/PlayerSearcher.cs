using UnityEngine;

public class PlayerSearcher : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;

    public float detectionRadius;
    [SerializeField] private float runningVelocity;
    [SerializeField] private float walkingVelocity;
    [SerializeField] private float runningWhenModifier;
    [SerializeField] private Transform playerReference;

    private float distanceToPlayer;
    private bool isWalking;
    private bool isRunning;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
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
        if (distanceToPlayer <= detectionRadius && distanceToPlayer > detectionRadius / runningWhenModifier)
	    {
            isWalking = true;
            isRunning = false;
	    } else if (distanceToPlayer > 0 && distanceToPlayer <= detectionRadius / runningWhenModifier) 
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
            Vector3 directionToPlayer = (playerReference.position - transform.position).normalized;
            m_Rigidbody.MoveRotation(Quaternion.LookRotation(directionToPlayer));
            m_Rigidbody.MovePosition(transform.position + directionToPlayer * GetCurrentVelocity());
        }
    }

    private float GetCurrentVelocity()
    {
        if (isWalking && !isRunning) 
	    {
            return walkingVelocity;
	    } else if (isRunning && !isWalking)
	    {
            return runningVelocity; 
	    }
        return 0f;
    }
}
