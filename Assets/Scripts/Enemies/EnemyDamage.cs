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

    public void Start()
    {
        hitParticles.Stop();
    }

    public void Damage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        hitParticles.Stop();
        hitParticles.Play();
        if (health <= 0 + Mathf.Epsilon) Die();
    }

    public void Die()
    {
        Debug.Log("die");
        Instantiate(corpse, transform.position, transform.rotation);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(CoroutineUtil.FadeTo(GetComponent<MeshRenderer>(), 0.1f, 0, () => { Destroy(gameObject); }));
    }
}
