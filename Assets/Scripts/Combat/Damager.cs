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
            this.collides.Clear();
            this.damage = damage;
            this.collider.enabled = true;
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
            this.collider.enabled = false;
            this.collides.Clear();
        }));
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null) return;
        if (this.collides.Contains(damageable)) return;
        if (!damageable.DamagerTypes.Contains(this.type)) return;
        this.CauseDamage(damageable);
    }

    // Actually causes damage
    private void CauseDamage(IDamageable damageable)
    {
        damageable.Damage(this.damage, this.type);
        this.collides.Add(damageable);
    }
}
