using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private PlayerMode modeGivingDamage;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private GameObject corpse;
    private DamageTextEmitter textEmitter;
    public float Health { get { return this.health; } }
    public PlayerMode ModeGivingDamage { get { return this.modeGivingDamage; } }

    private void Start()
    {
        textEmitter = GetComponent<DamageTextEmitter>();
        hitParticles.Stop();
    }

    public void Damage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        hitParticles.Stop();
        hitParticles.Play();
        if (health <= 0 + Mathf.Epsilon) Die();
        textEmitter.AddDamage(damage);
    }

    public void Die()
    {
        Instantiate(corpse, transform.position, transform.rotation);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(CoroutineUtil.FadeTo(GetComponent<MeshRenderer>(), 0.1f, 0, () => { Destroy(gameObject); }));
    }
}
