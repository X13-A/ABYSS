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
    [SerializeField] private GameObject selectedBlock;


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
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void MeleeAttack(PlayerAttackEvent e)
    {
        meleeDamager.Damage(meleeDamage, meleeDuration);
        EventManager.Instance.Raise(new AnimateItemEvent
        {
            itemId = "PlayerSword",
            animations = new Dictionary<string, float>
            {
                { "startTrail", 0f },
                { "stopTrail", e.duration * meleeStartPercentage }
            }
        });
    }

    private void WandAttack(PlayerAttackEvent e)
    {
        EventManager.Instance.Raise(new AnimateItemEvent
        {
            itemId = "PlayerWand",
            animations = new Dictionary<string, float>
            {
                { "startTrail", 0f },
                { "stopTrail", e.duration * wandStartPercentage },
                { "startWandCast", e.duration * wandStartPercentage }
            }
        });

        StartCoroutine(CoroutineUtil.DelayAction(e.duration * wandStartPercentage, () =>
        {
            Projectile projectile = Instantiate(magicProjectilePrefab, magicProjectilePosition.transform.position, Quaternion.Euler(
                magicProjectilePosition.transform.rotation.eulerAngles.x,
                magicProjectilePosition.transform.rotation.eulerAngles.y,
                magicProjectilePosition.transform.rotation.eulerAngles.z)
            ).GetComponent<Projectile>();
            projectile.Init(magicProjectileSpeed, magicProjectileDamage);
        }));
    }

    private void PickaxeAttack(PlayerAttackEvent e)
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(this.pickaxeDamage, AttackType.PICKAXE);
        }
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
        else if (e.type == AttackType.PICKAXE)
        {
            PickaxeAttack(e);
        }
    }

    private void Build()
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            GameObject currentCube = Instantiate(selectedBlock);
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

    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }

        // Build
        if (PlayerManager.Instance.ActivePlayerMode == PlayerMode.BUILD)
        {
            if (Input.GetButtonDown("Fire1") && PlayerManager.Instance.ActiveAimingMode == AimingMode.CURSOR)
            {
                Build();
            }
            return;
        }

        // Damage
        if (Input.GetButtonDown("Fire1") && AttackElaspedTime > currentAttackDuration && PlayerManager.Instance.ActivePlayerMode != PlayerMode.UNARMED)
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
            else if (ActiveAttackMode == AttackType.PICKAXE)
            {
                currentAttackDuration = pickaxeDuration;
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.PICKAXE,
                    damage = pickaxeDamage,
                    duration = pickaxeDuration,
                });
            }
            attackStartTime = Time.time;
        }
    }
}
