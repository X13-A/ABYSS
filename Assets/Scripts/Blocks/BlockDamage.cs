using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private PlayerMode modeGivingDamage;
    [SerializeField] private Animation animationHit;

    public float Health { get { return this.health; } }
    public PlayerMode ModeGivingDamage { get { return this.modeGivingDamage; } }

    void Start()
    {
    }

    public void Damage(float damage)
    {
        this.health = Mathf.Max(0, health - damage);
        this.animationHit.Play();
        if (this.health <= 0 + Mathf.Epsilon) this.Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
