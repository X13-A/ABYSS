using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Damager : MonoBehaviour, IDamager
{
    [SerializeField] private float damage;
    private new Collider collider;
    private AttackType type = AttackType.MELEE;
    public AttackType Type { get { return type; } }
    public HashSet<IDamageable> collides = new HashSet<IDamageable>();

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Inflige des dégats si l'ennemi n'a pas déjà été touché
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !collides.Contains(damageable))
        {
            damageable.Damage(damage);
            collides.Add(damageable);
        }
    }

    public void Damage(float damage, float duration)
    {
        // Enable
        StartCoroutine(CoroutineUtil.DelayAction(duration * 0.4f, () =>
        {
            collides.Clear();
            this.damage = damage;
            collider.enabled = true;
        }));

        // Never disable (arrows, projectiles)
        if (duration <= 0) return;
        
        // Disable
        StartCoroutine(CoroutineUtil.DelayAction(duration, () =>
        {
            this.damage = 0;
            collider.enabled = false;
            collides.Clear();
        }));
    }
}
