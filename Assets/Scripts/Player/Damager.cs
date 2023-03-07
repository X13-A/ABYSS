using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour, IDamager, IEventHandler
{
    float damage;
    float duration;
    float startTime;
    new Collider collider;
    AttackType type = AttackType.Melee;

    public AttackType Type { get { return type; } }

    public HashSet<IDamageable> collides = new HashSet<IDamageable>();

    private void Awake()
    {
        startTime = Time.time - 1000;
        collider = GetComponent<Collider>();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerAttackEvent>(Activate);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerAttackEvent>(Activate);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
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

    public void Activate(PlayerAttackEvent e)
    {
        if (e.type != type) return;
        damage = e.damage;
        duration = e.duration;
        startTime = Time.time;
        collider.enabled = true;
    }

    public void Deactivate()
    {
        collider.enabled = false;
        collides.Clear();
    }

    void Update()
    {
        if (Time.time - startTime >= duration && collider.enabled)
        {
            Deactivate();
        }
    }
}
