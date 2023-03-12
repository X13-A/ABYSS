using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] bool AOE;
    [SerializeField] bool destroyOnCollision;
    [SerializeField] GameObject damager;
    [SerializeField] GameObject collideEffect; // Object created when projectile collides with something
    float damage;
    float speed;

    public void Init(float speed, float damage, float lifespan = 2)
    {
        this.speed = speed;
        this.damage = damage;
        if (!AOE)
        {
            damager.GetComponent<Damager>().Damage(damage, 0);
        }
        Invoke("Die", lifespan);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.destroyOnCollision && other.GetComponent<StopProjectiles>() != null)
        {
            Die();
        }
    }

    void Die()
    {
        if (this.collideEffect != null)
        {
            GameObject effectInstance = Instantiate(this.collideEffect, this.transform.position, this.transform.rotation);
            if (AOE)
            {
                // Activate damage on explosion object
                this.damager.transform.SetParent(effectInstance.transform);
                this.damager.GetComponent<Damager>().Damage(this.damage, 0);
            }
        }
        Destroy(this.gameObject);
    }

    void Update()
    {
        this.transform.position += Vector3.forward * this.speed;
    }
}
