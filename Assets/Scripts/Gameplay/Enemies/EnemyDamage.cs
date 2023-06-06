using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private GameObject corpse;
    private DamageTextEmitter textEmitter;
    public float Health { get { return health; } }
    public List<AttackType> DamagerTypes { get { return damagerTypes; } }
    public List<float> DamagerTypesFactors { get { return damagerTypesFactors; } }

    private void Start()
    {
        textEmitter = GetComponent<DamageTextEmitter>();
        hitParticles.Stop();
    }

    public void Damage(float damage, AttackType type)
    {
        // Scale damage according to factors
        int typeIndex = damagerTypes.IndexOf(type);
        float damageFactor = damagerTypesFactors[typeIndex];
        float scaledDamage = damage * damageFactor;

        health = Mathf.Max(0, health - scaledDamage);
        hitParticles.Stop();
        hitParticles.Play();
        if (health <= 0 + Mathf.Epsilon) Die();
        textEmitter.AddDamage(scaledDamage);
    }

    public void Die()
    {
        Instantiate(corpse, transform.position, transform.rotation);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(CoroutineUtil.FadeTo(GetComponentInChildren<SkinnedMeshRenderer>(), 0.1f, 0, () => { Destroy(gameObject); }));
    }
}
