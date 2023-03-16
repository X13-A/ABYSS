using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AttackType { MELEE, WAND };
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

    [SerializeField] AttackType activeAttackType;
    public AttackType ActiveAttackType
    {
        get { return activeAttackType; }
        set 
        {
            activeAttackType = value;
            CursorManager.Instance.SetCursorType(this.CursorTypeFromAttackType(value));
        }
    }

    float currentAttackDuration;
    float attackStartTime;
    public float AttackElaspedTime 
    {
        get { return Time.time - attackStartTime; } 
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
        this.attackStartTime = Time.time - 1000;
        this.currentAttackDuration = 0;
        this.meleeTrail.Stop(true); // true makes the child animations stop too
        this.wandTrail.Stop(true);
        this.wandCast.Stop(true);
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    CursorType CursorTypeFromAttackType(AttackType type)
    {
        return type switch
        {
            AttackType.MELEE => CursorType.MELEE,
            AttackType.WAND => CursorType.MAGIC,
            _ => CursorType.MELEE,
        };
    }

    void MeleeAttack(PlayerAttackEvent e)
    {
        this.meleeDamager.Damage(this.meleeDamage, this.meleeDuration);
        this.meleeTrail.Play(true);
        StartCoroutine(CoroutineUtil.DelayAction(e.duration * this.meleeStartPercentage, () => { this.meleeTrail.Stop(); }));
    }

    void WandAttack(PlayerAttackEvent e)
    {
        this.wandTrail.Play();
        StartCoroutine(CoroutineUtil.DelayAction(e.duration * this.wandStartPercentage, () => {
            this.wandTrail.Stop(true);
            this.wandCast.Stop(true);
            this.wandCast.Play(true);
            Projectile projectile = Instantiate(this.magicProjectilePrefab, this.magicProjectilePosition.transform.position, Quaternion.Euler(
            this.magicProjectilePosition.transform.rotation.eulerAngles.x,
            this.magicProjectilePosition.transform.rotation.eulerAngles.y,
            this.magicProjectilePosition.transform.rotation.eulerAngles.z)).GetComponent<Projectile>();
            projectile.Init(this.magicProjectileSpeed, this.magicProjectileDamage);
        }));
    }

    void Attack(PlayerAttackEvent e)
    {
        if (e.type == AttackType.MELEE) MeleeAttack(e);
        else if (e.type == AttackType.WAND) WandAttack(e);
    }

    void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;


        if (Input.GetButtonDown("Quick Melee")) this.ActiveAttackType = AttackType.MELEE;
        if (Input.GetButtonDown("Quick Magic")) this.ActiveAttackType = AttackType.WAND;

        if (Input.GetButtonDown("Fire1") && this.AttackElaspedTime > this.currentAttackDuration)
        {
            if (this.activeAttackType == AttackType.MELEE)
            {
                this.currentAttackDuration = this.meleeDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MELEE,
                    damage = this.meleeDamage,
                    duration = this.meleeDuration,
                });
            }
            else if (this.activeAttackType == AttackType.WAND)
            {
                this.currentAttackDuration = this.wandDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.WAND,
                    damage = this.magicProjectileDamage,
                    duration = this.wandDuration,
                });
            }
            this.attackStartTime = Time.time;
        }
    }
}
