using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool AOE;
    [SerializeField] private bool destroyOnCollision;
    [SerializeField] private GameObject damager;
    [SerializeField] private GameObject collideEffect; // Object created when projectile collides with something
    private float damage;
    private float speed;

    public void Init(float speed, float damage, float lifespan = 2)
    {
        this.speed = speed;
        this.damage = damage;
        if (!AOE)
        {
            damager.GetComponent<Damager>().Damage(damage, 0);
        }
        Invoke(nameof(Die), lifespan);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        HandleCollision(other.collider);
    }

    private void HandleCollision(Collider other)
    {
        if (destroyOnCollision && other.GetComponent<StopProjectiles>() != null)
        {
            Die();
        }
    }

    private void Die()
    {
        if (collideEffect != null)
        {
            GameObject effectInstance = Instantiate(collideEffect, transform.position, transform.rotation);
            if (AOE)
            {
                // Activate damage on explosion object
                damager.transform.SetParent(effectInstance.transform);
                damager.GetComponent<Damager>().Damage(damage, 0);
            }
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * speed * Time.deltaTime;
    }
}
