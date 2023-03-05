using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }


    void oldAnimations()
    {
        if (m_Animator == null) return;

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        float movementMagnitude = Mathf.Clamp(Mathf.Sqrt(hInput * hInput + vInput * vInput), 0, 1);

        // States
        AnimatorStateInfo currentAnimation = m_Animator.GetCurrentAnimatorStateInfo(0);
        bool attacking = currentAnimation.IsName("Attack");
        bool moving = movementMagnitude >= float.Epsilon;

        // Let animation finish
        if (attacking) return;

        // Actions
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(attacking);
            m_Animator.speed = 1;
            m_Animator.SetTrigger("Attack");
            return;
        }

        // Movement
        if (moving)
        {
            m_Animator.speed = Mathf.Clamp(movementMagnitude, 0.25f, 1);
            if (!currentAnimation.IsName("Move"))
            {
                m_Animator.SetTrigger("Move");
            }
            return;
        }

        if (!currentAnimation.IsName("Idle"))
        {
            m_Animator.speed = 1;
            m_Animator.SetTrigger("Idle");
        }
    }
    private void Update()
    {
        // Disable animations on menus
        bool isIdling = m_Animator.GetBool("IsIdling");
        if (GameManager.Instance.State != GAMESTATE.play)
        {
            if (!isIdling)
            {
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetBool("IsIdling", true);
            }
            return;
        }

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        float movementMagnitude = Mathf.Clamp(Mathf.Sqrt(hInput * hInput + vInput * vInput), 0, 1);

        bool hasMovement = movementMagnitude >= float.Epsilon;
        bool isWalking = m_Animator.GetBool("IsWalking");

        if (!isWalking && hasMovement)
        {
            m_Animator.SetBool("IsWalking", true);
            m_Animator.SetBool("IsIdling", false);
        }

        if (!isIdling && !hasMovement)
        {
            m_Animator.SetBool("IsIdling", true);
            m_Animator.SetBool("IsWalking", false);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_Animator.SetTrigger("Attack");
        }
    }
}
