using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SDD.Events;

public class PlayerAnimationController : MonoBehaviour, IEventHandler
{
    private Animator m_Animator;

    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip meleeAttack;
    [SerializeField] private AnimationClip wandAttack;
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerAttackEvent>(HandleAttack);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerAttackEvent>(HandleAttack);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void HandleAttack(PlayerAttackEvent e)
    {
        if (e.type == AttackType.MELEE)
        {
            // Déclenche l'animation avec la bonne vitesse
            m_Animator.SetTrigger("Attack");
            float clipLength = meleeAttack.length;
            m_Animator.SetFloat("AttackSpeed", clipLength / e.duration);
        }
        else if (e.type == AttackType.MAGIC)
        {
            m_Animator.SetTrigger("Wand Attack");
            float clipLength = wandAttack.length;
            m_Animator.SetFloat("AttackSpeed", clipLength / e.duration);
        }
    }


    private void Update()
    {
        bool isIdling = m_Animator.GetBool("IsIdling");
        if (GameManager.Instance.State != GAMESTATE.PLAY)
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
    }
}
