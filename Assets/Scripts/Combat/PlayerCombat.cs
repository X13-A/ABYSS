using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AttackType { Melee, Wand };
public class PlayerCombat : MonoBehaviour, IEventHandler
{
    [SerializeField] Damager meleeDamager;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeDuration;
    [SerializeField] float meleeStartPercentage; // Percentage of the animation at which the damage actually occurs
    [SerializeField] ParticleSystem meleeTrail;

    [SerializeField] GameObject magicProjectilePrefab;
    [SerializeField] GameObject magicProjectilePosition;
    [SerializeField] float magicProjectileSpeed;
    [SerializeField] float magicProjectileDamage;
    [SerializeField] float wandDuration;
    [SerializeField] float wandStartPercentage; // Percentage of the animation at which the projectile is fired
    [SerializeField] ParticleSystem wandTrail;
    [SerializeField] ParticleSystem wandCast;

    [SerializeField] AttackType attackType;

    float currentAttackDuration;
    float attackStartTime;
    public float AttackElaspedTime 
    {
        get 
        {
            return Time.time - attackStartTime; 
        } 
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerAttackEvent>(Attack);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerAttackEvent>(Attack);
    }

    void OnEnable()
    {
        SubscribeEvents();
        // startTime is in the past by default to let the player attack when he just appeared
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        meleeTrail.Stop(true); // true makes the child animations stop too
        wandTrail.Stop(true);
        wandCast.Stop(true);
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Attack(PlayerAttackEvent e)
    {
        if (e.type == AttackType.Melee) 
        {
            meleeDamager.Damage(meleeDamage, meleeDuration);
            meleeTrail.Play(true);
            StartCoroutine(CoroutineUtil.DelayAction(e.duration * meleeStartPercentage, () => { meleeTrail.Stop(); }));
        }
        else if (e.type == AttackType.Wand)
        {
            wandTrail.Play();
            StartCoroutine(CoroutineUtil.DelayAction(e.duration * wandStartPercentage, () => {
                wandTrail.Stop(true);
                wandCast.Stop(true);
                wandCast.Play(true);
                Projectile projectile = Instantiate(magicProjectilePrefab, magicProjectilePosition.transform.position, Quaternion.Euler(
                magicProjectilePosition.transform.rotation.eulerAngles.x,
                magicProjectilePosition.transform.rotation.eulerAngles.y,
                magicProjectilePosition.transform.rotation.eulerAngles.z)).GetComponent<Projectile>();
                projectile.Init(magicProjectileSpeed, magicProjectileDamage);
            }));
        }
    }

    void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.play) return;

        if (Input.GetButtonDown("Fire1") && AttackElaspedTime > currentAttackDuration)
        {
            if (attackType == AttackType.Melee)
            {
                currentAttackDuration = meleeDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.Melee,
                    damage = meleeDamage,
                    duration = meleeDuration,
                });
            }
            else if (attackType == AttackType.Wand)
            {
                currentAttackDuration = wandDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.Wand,
                    damage = magicProjectileDamage,
                    duration = wandDuration,
                });
            }
            attackStartTime = Time.time;
        }
    }
}
