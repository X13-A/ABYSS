using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SDD.Events;
using System.Xml;
using System.ComponentModel;

[System.Serializable]
public class MyKeyValuePair
{
    public string name;
    public AnimationClip animationClip;
}

public class PlayerAnimationController : MonoBehaviour, IEventHandler
{
    private Animator m_Animator;


    [SerializeField] private MyKeyValuePair[] pseudoAnimationsDictionary;

    private Dictionary<string, AnimationClip> animations = new Dictionary<string, AnimationClip>();

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        foreach (var pair in pseudoAnimationsDictionary)
        {
            animations.Add(pair.name, pair.animationClip);
        }
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<AnimateAttackEvent>(HandleAttack);
        EventManager.Instance.AddListener<BossDefeatedEvent>(Die);
        EventManager.Instance.AddListener<PlayerDeadEvent>(Die);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<AnimateAttackEvent>(HandleAttack);
        EventManager.Instance.RemoveListener<BossDefeatedEvent>(Die);
        EventManager.Instance.RemoveListener<PlayerDeadEvent>(Die);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void HandleAttack(AnimateAttackEvent e)
    {
        m_Animator.SetTrigger(e.name);
        AnimationClip animation = animations[e.name];
        if (animation == null) return;
        float clipLength = animation.length;
        m_Animator.SetFloat("AttackSpeed", clipLength / e.animationDuration);
    }

    private void Die(BossDefeatedEvent e)
    {
        m_Animator.speed = 0.2f;
        m_Animator.SetTrigger("IsDefeated");
    }

    private void Die(PlayerDeadEvent e)
    {
        m_Animator.SetTrigger("IsDefeated");
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
            EventManager.Instance.Raise(new PlayerMoveEvent { isMoving = true });
        }

        if (!isIdling && !hasMovement)
        {
            m_Animator.SetBool("IsIdling", true);
            m_Animator.SetBool("IsWalking", false);
            EventManager.Instance.Raise(new PlayerMoveEvent { isMoving = false });
        }
    }
}
