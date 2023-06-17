using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseBoxDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private Animation animationHit;
    [SerializeField] GameObject[] spawnObjects;
    [SerializeField] private int maxDroppedObjects = 5;
    public DamageableType DamageableType => DamageableType.Decor;

    public float Health { get { return health; } set { health = value; } }
    public List<AttackType> DamagerTypes { get { return damagerTypes; } }
    public List<float> DamagerTypesFactors { get { return damagerTypesFactors; } }

    public void OnEnable()
    {
        animationHit = GetComponent<Animation>();
    }

    public void Damage(float damage, AttackType type)
    {
        Debug.Log("DAMAGE");
        if (health < 0 + Mathf.Epsilon) return;

        int factorIndex = damagerTypes.IndexOf(type);
        float scaledDamage = damage;
        if (factorIndex != -1 && factorIndex < damagerTypesFactors.Count)
        {
            scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];
        }

        health = Mathf.Max(0, health - scaledDamage);
        animationHit.Play("BoxCrash");
        if (health <= 0 + Mathf.Epsilon) Die();
    }

    public void Die()
    {
        // Remove colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        if (spawnObjects.Length != 0)
        {
            int droppedObjects = Random.Range(1, maxDroppedObjects);
            for (int i = 0; i < droppedObjects; i++)
            {
                int randomIndex = Random.Range(0, spawnObjects.Length);
                GameObject spawnedObject = Instantiate(spawnObjects[randomIndex], transform.position, transform.rotation);

                Rigidbody spawnedRigidbody = spawnedObject.GetComponent<Rigidbody>();
                if (spawnedRigidbody != null)
                {
                    Vector3 forceDirection = Vector3.up;
                    float forceMagnitude = 6.0f;

                    spawnedRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                }
            }
        }
        yield return new WaitForSeconds(animationHit.clip.length);
        Destroy(gameObject);
    }
}
