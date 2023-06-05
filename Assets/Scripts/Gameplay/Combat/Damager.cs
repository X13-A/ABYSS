using SDD.Events;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour, IDamager
{
    [SerializeField] private float damage;
    [SerializeField] private AttackType type;
    private new Collider collider;
    public AttackType Type => type;
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    // Enables damage
    public void Damage(float damage, float duration)
    {
        type = AttackType.MELEE;
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(duration * 0.4f, () =>
        {
            collides.Clear();
            this.damage = damage;
            collider.enabled = true;
        }));

        // Never disable (arrows, projectiles)
        if (duration <= 0)
        {
            return;
        }

        // Disable
        StartCoroutine(CoroutineUtil.DelayAction(duration, () =>
        {
            this.damage = 0;
            collider.enabled = false;
            collides.Clear();
        }));
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null) return;
        if (collides.Contains(damageable)) return;
        if (!damageable.DamagerTypes.Contains(type)) return;
        CauseDamage(damageable);
    }

    // Actually causes damage
    private void CauseDamage(IDamageable damageable)
    {
        damageable.Damage(damage, type);
        collides.Add(damageable);
    }
}
