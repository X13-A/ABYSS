using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IUseItem
{
    [SerializeField] private float meleeDuration;
    [SerializeField] private float meleeStartPercentage;
    [SerializeField] private float SwordDamage;

    private float instantDamage;
    private SwordAnimation swordAnimation;
    private AudioSource audioSource;

    private MeshCollider collider;
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();
    private AttackType attackType;

    private float currentAttackDuration;
    private float attackStartTime;

    public float AttackElaspedTime => Time.time - attackStartTime;

    private void OnEnable()
    {
        collider = GetComponent<MeshCollider>();
        attackType = AttackType.MELEE;
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        swordAnimation = GetComponent<SwordAnimation>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage()
    {
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(meleeDuration * 0.4f, () =>
        {
            collides.Clear();
            instantDamage = SwordDamage;
            collider.enabled = true;
        }));

        // Never disable (arrows, projectiles)
        if (meleeDuration <= 0)
        {
            return;
        }

        // Disable
        StartCoroutine(CoroutineUtil.DelayAction(meleeDuration, () =>
        {
            this.instantDamage = 0;
            collider.enabled = false;
            collides.Clear();
        }));
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;
        if (collides.Contains(damageable)) return;
        if (!damageable.DamagerTypes.Contains(attackType)) return;
        damageable.Damage(instantDamage, attackType);
        collides.Add(damageable);
    }
        

    public void Use()
    {
        if (AttackElaspedTime < currentAttackDuration) return;
        currentAttackDuration = meleeDuration;
        attackStartTime = Time.time;

        Damage();

        audioSource.PlayOneShot(audioSource.clip);
        EventManager.Instance.Raise(new AnimateAttackEvent
        {
            name = "Attack",
            animationDuration = meleeDuration
        });
        swordAnimation.Animate(meleeDuration * currentAttackDuration);
    }
}
