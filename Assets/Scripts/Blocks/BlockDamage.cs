using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private Animation animationHit;

    public float Health { get { return this.health; } }
    public List<AttackType> DamagerTypes { get { return damagerTypes; } }
    public List<float> DamagerTypesFactors { get { return damagerTypesFactors; } }

    public void Update()
    {
        if (this.health > 0 + Mathf.Epsilon) return;
        transform.Rotate(Vector3.up * Time.deltaTime * 100);
    }

    public void Damage(float damage, AttackType type)
    {
        // Scale damage according to factors
        //Debug.Log(damagerTypes.IndexOf(type));
        if (this.health < 0 + Mathf.Epsilon) return;
        float scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];

        this.health = Mathf.Max(0, health - scaledDamage);
        this.animationHit.Play("ChangeScalingHit");
        Debug.Log(damage);
        if (this.health <= 0 + Mathf.Epsilon) this.Die();
    }

    public void Die()
    {
        this.animationHit.Play("ChangeScalingDestruction");
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
}
