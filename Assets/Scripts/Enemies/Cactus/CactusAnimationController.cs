using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAnimationController : MonoBehaviour, IEventHandler
{
    private Animator animator;
    private Rigidbody rb;
    private EnemyAI enemyAI;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<CactusAttackEvent>(HandleAttack);
    }

    public void UnsubscribeEvents()
    { 
        EventManager.Instance.RemoveListener<CactusAttackEvent>(HandleAttack);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        SubscribeEvents();
    }

    private void HandleAttack(CactusAttackEvent e)
    {
        animator.SetTrigger("attack");
    }

}
