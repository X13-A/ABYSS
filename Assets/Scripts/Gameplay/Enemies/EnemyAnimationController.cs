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

        // We use the original RuntimeAnimatorController as our base
        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;

        if (baseController != null)
        {
            // Create a new AnimatorOverrideController based on the original
            AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);

            // Use the new AnimatorOverrideController
            animator.runtimeAnimatorController = overrideController;
        }
        else
        {
            Debug.LogError("Animator's runtimeAnimatorController is null.");
        }
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
