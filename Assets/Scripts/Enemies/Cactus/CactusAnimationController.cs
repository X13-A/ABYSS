using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAnimationController : MonoBehaviour 
{
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private PlayerSearcher m_PlayerSearcher;

    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip run;

    private float runningWhenModifier = 1.3f;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerSearcher = GetComponent<PlayerSearcher>();
    }

    private void Update()
    {
        Vector3 velocity = m_Rigidbody.velocity;
        bool hasMovement = velocity.x > float.Epsilon;
        float distanceToPlayer = m_Animator.GetFloat("distanceToPlayer");
        float detectionRadius = m_PlayerSearcher.detectionRadius;
        if (hasMovement && distanceToPlayer >= detectionRadius / runningWhenModifier && distanceToPlayer <= detectionRadius) // running
        {
            PlayerSearcher.velocity = 
            m_Animator.SetBool("isWalking", true);
        }
        else if (hasMovement && distanceToPlayer < detectionRadius / runningWhenModifier && distanceToPlayer > 0) { // walking
            m_Animator.SetBool("isRunning", true);
    	}
    }
}
