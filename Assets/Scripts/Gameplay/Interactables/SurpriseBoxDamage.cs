using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseBoxDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private Animation animationHit;
    [SerializeField] private List<ItemId> spawnItems;
    [SerializeField] private int maxDroppedObjects = 5;
    [SerializeField] private float throwAngle = 75;
    [SerializeField] private float throwForce = 5;
    [SerializeField] private float torqueForce = 5;

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
        if (spawnItems.Count != 0)
        {
            int droppedObjects = Random.Range(1, maxDroppedObjects);
            for (int i = 0; i < droppedObjects; i++)
            {
                int randomIndex = Random.Range(0, spawnItems.Count);
                ThrowItem(spawnItems[randomIndex]);
            }
        }
        yield return new WaitForSeconds(animationHit.clip.length);
        Destroy(gameObject);
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
        float x = Random.Range(-1, 1);
        float y = Random.Range(-1, 1);
        float z = Random.Range(-1, 1);
        return new Vector3(x, y, z).normalized;
    }

    private Vector3 RandomizeDirection()
    {
        float xzAngle = Random.Range(0f, 360f); // Random horizontal angle
        float yRadians = throwAngle * Mathf.Deg2Rad;
        float xzRadians = xzAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(yRadians) * Mathf.Cos(xzRadians);
        float y = Mathf.Cos(yRadians);
        float z = Mathf.Sin(yRadians) * Mathf.Sin(xzRadians);
        return new Vector3(x, y, z).normalized;
    }
}
