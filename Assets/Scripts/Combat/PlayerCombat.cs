using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour, IEventHandler
{
    [SerializeField] private Damager meleeDamager;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeDuration;
    [SerializeField] private float meleeStartPercentage; // Percentage of the animation at which the damage actually occurs
    [SerializeField] private ParticleSystem meleeTrail;

    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] private GameObject magicProjectilePosition;
    [SerializeField] private float magicProjectileSpeed;
    [SerializeField] private float magicProjectileDamage;
    [SerializeField] private float wandDuration;
    [SerializeField] private float wandStartPercentage; // Percentage of the animation at which the projectile is fired
    [SerializeField] private ParticleSystem wandTrail;
    [SerializeField] private ParticleSystem wandCast;

    [SerializeField] private AttackType activeAttackMode;
    public AttackType ActiveAttackMode
    {
        get { return activeAttackMode; }
        set 
        {
            activeAttackMode = value;
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = EnumConverter.PlayerModeFromAttackType(value) });
        }
    }

    private float currentAttackDuration;
    private float attackStartTime;
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

    private void OnEnable()
    {
        SubscribeEvents();
        // startTime is in the past by default to let the player attack when he just appeared
        this.attackStartTime = Time.time - 1000;
        this.currentAttackDuration = 0;
        this.meleeTrail.Stop(true); // true makes the child animations stop too
        this.wandTrail.Stop(true);
        this.wandCast.Stop(true);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void MeleeAttack(PlayerAttackEvent e)
    {
        this.meleeDamager.Damage(this.meleeDamage, this.meleeDuration);
        this.meleeTrail.Play(true);
        StartCoroutine(CoroutineUtil.DelayAction(e.duration * this.meleeStartPercentage, () => { this.meleeTrail.Stop(); }));
    }

    private void WandAttack(PlayerAttackEvent e)
    {
        this.wandTrail.Play();
        StartCoroutine(CoroutineUtil.DelayAction(e.duration * this.wandStartPercentage, () => {
            this.wandTrail.Stop(true);
            this.wandCast.Stop(true);
            this.wandCast.Play(true);
            Projectile projectile = Instantiate(this.magicProjectilePrefab, this.magicProjectilePosition.transform.position, Quaternion.Euler(
                this.magicProjectilePosition.transform.rotation.eulerAngles.x,
                this.magicProjectilePosition.transform.rotation.eulerAngles.y,
                this.magicProjectilePosition.transform.rotation.eulerAngles.z)
            ).GetComponent<Projectile>();
            projectile.Init(this.magicProjectileSpeed, this.magicProjectileDamage);
        }));
    }

    private void Attack(PlayerAttackEvent e)
    {
        if (e.type == AttackType.MELEE) MeleeAttack(e);
        else if (e.type == AttackType.MAGIC) WandAttack(e);
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;

        if (Input.GetButtonDown("Quick Melee")) this.ActiveAttackMode = AttackType.MELEE;
        if (Input.GetButtonDown("Quick Magic")) this.ActiveAttackMode = AttackType.MAGIC;

        if (PlayerManager.Instance.ActivePlayerMode == PlayerMode.BUILD) return;

        if (Input.GetButtonDown("Fire1") && this.AttackElaspedTime > this.currentAttackDuration)
        {
            if (this.ActiveAttackMode == AttackType.MELEE)
            {
                this.currentAttackDuration = this.meleeDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MELEE,
                    damage = this.meleeDamage,
                    duration = this.meleeDuration,
                });
            }
            else if (this.ActiveAttackMode == AttackType.MAGIC)
            {
                this.currentAttackDuration = this.wandDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MAGIC,
                    damage = this.magicProjectileDamage,
                    duration = this.wandDuration,
                });
            }
            this.attackStartTime = Time.time;
        }
    }
}
