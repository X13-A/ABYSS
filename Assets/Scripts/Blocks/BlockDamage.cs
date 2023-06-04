using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private Animation animationHit;

    public float Health { get { return health; } set { health = value; } }
    public List<AttackType> DamagerTypes { get { return damagerTypes; } }
    public List<float> DamagerTypesFactors { get { return damagerTypesFactors; } }

    public void OnEnable()
    {
        animationHit = GetComponent<Animation>();
    }

    public void Update()
    {
        if (health > 0 + Mathf.Epsilon) return;
        transform.Rotate(Vector3.up * Time.deltaTime * 100);
    }

    public void Damage(float damage, AttackType type)
    {
        // Scale damage according to factors
        //Debug.Log(damagerTypes.IndexOf(type));
        if (health < 0 + Mathf.Epsilon) return;
        float scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];

        health = Mathf.Max(0, health - scaledDamage);
        animationHit.Play("ChangeScalingHit");
        Debug.Log(damage);
        if (health <= 0 + Mathf.Epsilon) Die();
    }

    public void Die()
    {
        animationHit.Play("ChangeScalingDestruction");
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
}
