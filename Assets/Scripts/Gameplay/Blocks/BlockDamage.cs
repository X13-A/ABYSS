using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private ItemId id;
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    private Animation animationHit;

    public float Health => health;
    public List<AttackType> DamagerTypes => damagerTypes;
    public List<float> DamagerTypesFactors => damagerTypesFactors;

    public void OnEnable()
    {
        animationHit = GetComponent<Animation>();
    }

    public void Damage(float damage, AttackType type)
    {
        // Scale damage according to factors
        if (health < 0 + Mathf.Epsilon) return;
        float scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];

        health = Mathf.Max(0, health - scaledDamage);
        animationHit.Play("ChangeScalingHit");
        if (health <= 0 + Mathf.Epsilon) Die();
    }

    public void Die()
    {
        Instantiate(ItemBank.GetDroppedPrefab(id), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
