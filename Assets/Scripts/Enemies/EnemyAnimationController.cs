using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour, IEventHandler
{
    private Animator animator;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EnemyAttackEvent>(HandleAttack);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EnemyAttackEvent>(HandleAttack);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        SubscribeEvents();
    }

    private void Update()
    {
        UpdateAnimationState("runningAnimationPlaying", "isRunning");
        UpdateAnimationState("walkingAnimationPlaying", "isWalking");
    }

    private void UpdateAnimationState(string animating, string action)
    {
        bool isAnimating = animator.GetBool(animating);
        bool actionActive = animator.GetBool(action);
        if (!isAnimating && actionActive)
        {
            animator.SetBool(animating, true);
        }
        else
        {
            if (!actionActive)
            {
                animator.SetBool(animating, false);
            }
        }
    }

    private void HandleAttack(EnemyAttackEvent e)
    {
        animator.SetTrigger("attack");
    }
}
