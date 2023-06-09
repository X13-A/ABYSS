using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sword : MonoBehaviour, IUseItem
{
    [SerializeField] private float baseAttackDuration;
    [SerializeField] private float damageStartPercentage = 0.4f;
    [SerializeField] private float damageEndPercentage = 0.8f;
    [SerializeField] private float damage = 10;
    [SerializeField] private Damager damager;

    private SwordAnimation swordAnimation;
    private AudioSource audioSource;

    private float currentAttackDuration;
    private float attackStartTime;

    public float AttackElaspedTime => Time.time - attackStartTime;

    private void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        swordAnimation = GetComponent<SwordAnimation>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage()
    {
        damager.EnableDamage(damage, currentAttackDuration, damageStartPercentage, damageEndPercentage);
    }
        
    public bool Use()
    {
        if (AttackElaspedTime < currentAttackDuration) return false;

        currentAttackDuration = baseAttackDuration / PlayerManager.Instance.PlayerAttackSpeedMultiplier;
        attackStartTime = Time.time;

        Damage();

        audioSource.PlayOneShot(audioSource.clip);
        EventManager.Instance.Raise(new AnimateAttackEvent
        {
            name = "Attack",
            animationDuration = currentAttackDuration
        });
        swordAnimation.Animate(baseAttackDuration * currentAttackDuration);
        return true;
    }
}
