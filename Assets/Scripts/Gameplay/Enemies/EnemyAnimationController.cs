using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = baseController;
        animator.runtimeAnimatorController = overrideController;
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

    public void TriggerAttack()
    {
        animator.SetTrigger("attack");
    }
}
