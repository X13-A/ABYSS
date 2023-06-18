using System.Collections.Generic;
using UnityEngine;
using System;
using SDD.Events;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private GameObject corpse;
    [SerializeField] private float fadeOutTime = 0.1f;
    [SerializeField] private List<ItemId> droppedItems;
    [SerializeField] private List<float> droppedItemsProbality;

    [SerializeField] private float dropAngle = 75;
    [SerializeField] private float throwForce = 5;
    [SerializeField] private float torqueForce = 5;

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
        DropItems();
        EventManager.Instance.Raise(new SetScoreEvent { addedScore = 10 });
    }

    private void DropItems()
    {
        if (droppedItems.Count == 0) return;
        if (droppedItems.Count != droppedItemsProbality.Count) return;

        for (int i = 0; i < droppedItems.Count; i++)
        {
            float probability = (float) UnityEngine.Random.Range(0, 100) / 100;
            if (probability < droppedItemsProbality[i])
            {
                ThrowItem(droppedItems[i]);
            }
        }
    }

    private void ThrowItem(ItemId item)
    {
        GameObject droppedPrefab = ItemBank.GetDroppedPrefab(item);
        GameObject droppedGameObject = Instantiate(droppedPrefab, transform.position, Quaternion.identity);
        GameObject particles = ItemBank.GetDroppedParticles(item);

        GameObject particlesInstance = Instantiate(particles, droppedGameObject.transform.position, Quaternion.identity);
        particlesInstance.transform.SetParent(droppedGameObject.transform, false);
        particlesInstance.transform.localPosition = Vector3.zero;

        Rigidbody rigidbody = droppedGameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.useGravity = true;
            rigidbody.AddForce(RandomizeDirection() * throwForce, ForceMode.Impulse);
            rigidbody.AddTorque(RandomizeTorque() * torqueForce, ForceMode.Impulse);
        }

        if (droppedGameObject.GetComponent<DroppedItem>() == null)
        {
            droppedGameObject.AddComponent<DroppedItem>().Init(item);
        }

        droppedGameObject.transform.localScale = Vector3.zero;
        StartCoroutine(CoroutineUtil.ScaleTo(droppedGameObject.transform, 0.5f, new Vector3(1, 1, 1)));
    }

    private Vector3 RandomizeTorque()
    {
        float x = UnityEngine.Random.Range(-1, 1);
        float y = UnityEngine.Random.Range(-1, 1);
        float z = UnityEngine.Random.Range(-1, 1);
        return new Vector3(x, y, z).normalized;
    }

    private Vector3 RandomizeDirection()
    {
        float xzAngle = UnityEngine.Random.Range(0f, 360f); // Random horizontal angle
        float yRadians = dropAngle * Mathf.Deg2Rad;
        float xzRadians = xzAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(yRadians) * Mathf.Cos(xzRadians);
        float y = Mathf.Cos(yRadians);
        float z = Mathf.Sin(yRadians) * Mathf.Sin(xzRadians);
        return new Vector3(x, y, z).normalized;
    }
}
