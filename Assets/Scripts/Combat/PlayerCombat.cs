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
        get => activeAttackMode;
        set
        {
            activeAttackMode = value;
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = EnumConverter.PlayerModeFromAttackType(value) });
        }
    }

    private float currentAttackDuration;
    private float attackStartTime;
    public float AttackElaspedTime => Time.time - attackStartTime;

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
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        meleeTrail.Stop(true); // true makes the child animations stop too
        wandTrail.Stop(true);
        wandCast.Stop(true);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void MeleeAttack(PlayerAttackEvent e)
    {
        meleeDamager.Damage(meleeDamage, meleeDuration);
        //meleeTrail.Play(true);
        //StartCoroutine(CoroutineUtil.DelayAction(e.duration * meleeStartPercentage, () => meleeTrail.Stop()));
    }

    private void WandAttack(PlayerAttackEvent e)
    {
        wandTrail.Play();
        StartCoroutine(CoroutineUtil.DelayAction(e.duration * wandStartPercentage, () => {
            wandTrail.Stop(true);
            wandCast.Stop(true);
            wandCast.Play(true);
            Projectile projectile = Instantiate(magicProjectilePrefab, magicProjectilePosition.transform.position, Quaternion.Euler(
                magicProjectilePosition.transform.rotation.eulerAngles.x,
                magicProjectilePosition.transform.rotation.eulerAngles.y,
                magicProjectilePosition.transform.rotation.eulerAngles.z)
            ).GetComponent<Projectile>();
            projectile.Init(magicProjectileSpeed, magicProjectileDamage);
        }));
    }

    private void Attack(PlayerAttackEvent e)
    {
        if (e.type == AttackType.MELEE)
        {
            MeleeAttack(e);
        }
        else if (e.type == AttackType.MAGIC)
        {
            WandAttack(e);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.MELEE && PlayerManager.Instance.ActivePlayerMode != PlayerMode.PICKAXE)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && AttackElaspedTime > currentAttackDuration)
        {
            if (ActiveAttackMode == AttackType.MELEE)
            {
                currentAttackDuration = meleeDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MELEE,
                    damage = meleeDamage,
                    duration = meleeDuration,
                });
            }
            else if (ActiveAttackMode == AttackType.MAGIC)
            {
                currentAttackDuration = wandDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MAGIC,
                    damage = magicProjectileDamage,
                    duration = wandDuration,
                });
            }
            attackStartTime = Time.time;
        }
    }
}
