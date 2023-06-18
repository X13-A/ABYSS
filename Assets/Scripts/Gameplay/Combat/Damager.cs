using SDD.Events;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour, IDamager
{

    private float damage;
    [SerializeField] private AttackType type;
    [SerializeField] private List<DamageableType> damageableTypes = new List<DamageableType> { DamageableType.Enemy, DamageableType.Decor };

    [SerializeField] private Collider damagerCollider;
    public Collider Collider => damagerCollider;
    public AttackType Type => type;
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();


    private void Start()
    {
        damagerCollider = GetComponent<Collider>();
    }

    // Enables damage
    public void EnableDamage(float damage, float duration, float startPercent = 0, float endPercent = 1)
    {
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(duration * startPercent, () =>
        {
            collides.Clear();
            this.damage = damage;
            damagerCollider.enabled = true;
        }));

        // Never disable (arrows, projectiles)
        if (duration <= 0)
        {
            return;
        }

        // Disable
        StartCoroutine(CoroutineUtil.DelayAction(duration * endPercent, () =>
        {
            this.damage = 0;
            damagerCollider.enabled = false;
            collides.Clear();
        }));
    }


    // Use these to manually enable and disable the damager instead of enabling it for a duration
    public void EnableDamage(float damage)
    {
        collides.Clear();
        this.damage = damage;
        damagerCollider.enabled = true;
    }

    public void StopDamage()
    {
        this.damage = 0;
        damagerCollider.enabled = false;
        collides.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null) return;
        if (collides.Contains(damageable)) return;
        if (!damageable.DamagerTypes.Contains(type)) return;
        if (!damageableTypes.Contains(damageable.DamageableType)) return;

        CauseDamage(damageable);
    }

    // Actually causes damage
    private void CauseDamage(IDamageable damageable)
    {
        damageable.Damage(damage, type);
        collides.Add(damageable);
    }
}
