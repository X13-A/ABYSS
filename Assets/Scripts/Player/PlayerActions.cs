using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerActions : MonoBehaviour, IEventHandler
{
    [SerializeField] private Damager meleeDamager;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeDuration;
    [SerializeField] private float meleeStartPercentage; // Percentage of the animation at which the damage actually occurs

    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] private GameObject magicProjectilePosition;
    [SerializeField] private float magicProjectileSpeed;
    [SerializeField] private float magicProjectileDamage;
    [SerializeField] private float wandDuration;
    [SerializeField] private float wandStartPercentage; // Percentage of the animation at which the projectile is fired

    [SerializeField] private float pickaxeDamage;
    [SerializeField] private float pickaxeDuration;


    public AttackType ActiveAttackMode
    {
        get => EnumConverter.AttackTypeFromPlayerMode(PlayerManager.Instance.ActivePlayerMode);
        set
        {
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = EnumConverter.PlayerModeFromAttackType(value) });
        }
    }

    private float currentAttackDuration;
    private float attackStartTime;
    public float AttackElaspedTime => Time.time - attackStartTime;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerAttackEvent>(Attack);
        EventManager.Instance.AddListener<PlayerBuildEvent>(Build);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerAttackEvent>(Attack);
        EventManager.Instance.RemoveListener<PlayerBuildEvent>(Build);
    }

    private void OnEnable()
    {
        SubscribeEvents();
        // startTime is in the past by default to let the player attack when he just appeared
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void MeleeAttack(PlayerAttackEvent e)
    {
        this.meleeDamager.Damage(e.damage, e.hitDuration);
        EventManager.Instance.Raise(new AnimateItemEvent
        {
            itemId = ItemId.Sword,
            animations = new Dictionary<string, float>
            {
                { "startTrail", 0f },
                { "stopTrail", e.animationDuration * e.damageStartPercentage }
            }
        });
    }

    private void WandAttack(PlayerAttackEvent e)
    {
        EventManager.Instance.Raise(new AnimateItemEvent
        {
            itemId = ItemId.Wand,
            animations = new Dictionary<string, float>
            {
                { "startTrail", 0f },
                { "stopTrail", e.animationDuration * e.damageStartPercentage},
                { "startWandCast", e.animationDuration * e.damageStartPercentage }
            }
        });

        StartCoroutine(CoroutineUtil.DelayAction(e.animationDuration * e.damageStartPercentage, () =>
        {
            Projectile projectile = Instantiate(magicProjectilePrefab, magicProjectilePosition.transform.position, Quaternion.Euler(
                magicProjectilePosition.transform.rotation.eulerAngles.x,
                magicProjectilePosition.transform.rotation.eulerAngles.y,
                magicProjectilePosition.transform.rotation.eulerAngles.z)
            ).GetComponent<Projectile>();
            projectile.Init(e.projectileSpeed, e.damage);
        }));
    }

    private void PickaxeAttack(PlayerAttackEvent e)
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(e.damage, AttackType.PICKAXE);
        }
    }

    private void Attack(PlayerAttackEvent e)
    {
        if (this.AttackElaspedTime < this.currentAttackDuration) return;

        this.currentAttackDuration = e.cooldown;
        this.attackStartTime = Time.time;
        EventManager.Instance.Raise(new AnimateAttackEvent
        {
            animationDuration = e.animationDuration,
            type = e.type
        });

        if (e.type == AttackType.MELEE)
        {
            MeleeAttack(e);
        }
        else if (e.type == AttackType.MAGIC)
        {
            WandAttack(e);
        }
        else if (e.type == AttackType.PICKAXE)
        {
            PickaxeAttack(e);
        }
    }

    private void Build(PlayerBuildEvent e)
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            GameObject currentCube = Instantiate(e.block);
            currentCube.SetActive(true);
            currentCube.transform.position = hit.collider.transform.position + hit.normal;
            currentCube.transform.localScale = Vector3.one;
            currentCube.transform.rotation = Quaternion.identity;
            currentCube.GetComponent<BoxCollider>().enabled = true;
            currentCube.GetComponent<BoxCollider>().isTrigger = false;
            currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentCube.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
