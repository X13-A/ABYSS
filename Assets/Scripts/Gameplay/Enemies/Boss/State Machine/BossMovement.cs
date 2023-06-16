using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : StateMachineBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float attackRange = 2f;

    private Transform playerTransform;
    private Rigidbody bossRigidbody;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.playerTransform = PlayerManager.Instance.PlayerReference;
        this.bossRigidbody = BossManager.Instance.BossReference.GetComponent<Rigidbody>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;

        this.MoveAndRotateTowardPlayer();

        if (Vector3.Distance(playerTransform.position, bossRigidbody.transform.position) <= attackRange)
        {
            animator.SetTrigger("MeleeAttack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("MeleeAttack");
    }

    private void MoveAndRotateTowardPlayer()
    {
        Vector3 playerWidth = new Vector3(playerTransform.GetComponent<CharacterController>().radius, 0, 0);
        Vector3 bossWidth = GetBossHalfWidth();

        Vector3 directionToPlayer = ((PlayerManager.Instance.PlayerReference.position + playerWidth) - (bossRigidbody.transform.position + bossWidth)).normalized;
        directionToPlayer.y = 0; // remove any influence from the y axis
        Quaternion quaternionToPlayer = Quaternion.LookRotation(directionToPlayer);
        bossRigidbody.MoveRotation(Quaternion.Slerp(bossRigidbody.rotation, quaternionToPlayer, rotateSpeed * Time.deltaTime));
        bossRigidbody.MovePosition(bossRigidbody.transform.position + directionToPlayer * moveSpeed * Time.deltaTime);
    }

    private Vector3 GetBossHalfWidth()
    {
        Collider collider = this.bossRigidbody.GetComponent<Collider>();
        if (collider != null)
        {
            if (collider is CapsuleCollider capsule)
            {
                return new Vector3(capsule.radius, 0, 0);
            }
            if (collider is BoxCollider box)
            {
                return new Vector3(box.size.x / 2, 0, 0);
            }
        }
        return new Vector3(0, 0, 0);
    }
}
