using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAnimationController : MonoBehaviour, IEventHandler
{
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private EnemyAI m_EnemyAI;

    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip run;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<CactusAttackEvent>(HandleAttack);
    }

    public void UnsubscribeEvents()
    { 
        EventManager.Instance.RemoveListener<CactusAttackEvent>(HandleAttack);
    }

    private void HandleAttack(CactusAttackEvent e)
    {
        m_Animator.SetTrigger("attack");
    }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_EnemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
    }
}
