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
    [SerializeField] private float fadeOutTime = 0.1f;

    public DamageableType DamageableType => DamageableType.Enemy;

    private DamageTextEmitter textEmitter;
    public float Health { get { return health; } }
    public List<AttackType> DamagerTypes { get { return damagerTypes; } }
    public List<float> DamagerTypesFactors { get { return damagerTypesFactors; } }

    private void Start()
    {
        textEmitter = GetComponent<DamageTextEmitter>();
        if (hitParticles != null)
        {
            hitParticles.Stop();
        }
    }

    public void Damage(float damage, AttackType type)
    {
        // Scale damage according to factors
        int factorIndex = damagerTypes.IndexOf(type);
        float scaledDamage = damage;
        if (factorIndex != -1 && factorIndex < damagerTypesFactors.Count)
        {
            scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];
        }

        health = Mathf.Max(0, health - scaledDamage);
        if (hitParticles != null)
        {
            hitParticles.Stop();
            hitParticles.Play();
        }
        if (health <= 0 + Mathf.Epsilon) Die();
        if (textEmitter != null) textEmitter.AddDamage(scaledDamage);
    }

    public void Die()
    {
        if (corpse != null) Instantiate(corpse, transform.position, transform.rotation);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            StartCoroutine(CoroutineUtil.FadeTo(mesh, fadeOutTime, 0, () => { Destroy(gameObject); }));
        }
    }
}
