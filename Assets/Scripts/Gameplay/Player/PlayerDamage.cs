using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    public DamageableType DamageableType => DamageableType.Player;

    public float Health => PlayerManager.Instance.Health;

    [SerializeField] private List<AttackType> damagerTypes;
    [SerializeField] private List<float> damagerTypesFactors;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private GameObject animatedModel;
    private void Start()
    {
        if (hitParticles != null)
        {
            hitParticles.Stop();
        }
    }

    public List<AttackType> DamagerTypes => damagerTypes;
    public List<float> DamagerTypesFactors => damagerTypesFactors;

    public void Damage(float damage, AttackType type)
    {
        // Inflict damage
        int factorIndex = damagerTypes.IndexOf(type);
        float scaledDamage = damage;
        if (factorIndex != -1 && factorIndex < damagerTypesFactors.Count)
        {
            scaledDamage = damage * damagerTypesFactors[damagerTypes.IndexOf(type)];
        }
        EventManager.Instance.Raise(new DamagePlayerEvent { damage = scaledDamage });

        // Hit particles
        if (hitParticles != null)
        {
            hitParticles.Stop();
            hitParticles.Play();
        }
        StartCoroutine(CoroutineUtil.ScaleTo(animatedModel.transform, 0.1f, new Vector3(1.1f, 1.1f, 1.1f), () =>
        {
            StartCoroutine(CoroutineUtil.ScaleTo(animatedModel.transform, 0.1f, new Vector3(1f, 1f, 1f)));
        }));
    }

    public void Die()
    {

    }
}
