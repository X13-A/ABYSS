using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [SerializeField] float health;
    public float Health { get { return health; } }

    public void Damage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        if (health <= 0 + Mathf.Epsilon) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
