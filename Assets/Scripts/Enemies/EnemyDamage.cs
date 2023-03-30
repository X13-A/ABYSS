using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [SerializeField] float health;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] GameObject corpse;
    public float Health { get { return health; } }

    private void Start()
    {
        this.textEmitter = GetComponent<DamageTextEmitter>();
        this.hitParticles.Stop();
    }

    public void Damage(float damage)
    {
        this.health = Mathf.Max(0, this.health - damage);
        this.hitParticles.Stop();
        this.hitParticles.Play();
        if (this.health <= 0 + Mathf.Epsilon) Die();
        this.textEmitter.AddDamage(damage);
    }

    public void Die()
    {
        Instantiate(this.corpse, this.transform.position, this.transform.rotation);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(CoroutineUtil.FadeTo(GetComponent<MeshRenderer>(), 0.1f, 0, () => { Destroy(this.gameObject); }));
    }
}
