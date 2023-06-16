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

    public List<AttackType> DamagerTypes => damagerTypes;
    public List<float> DamagerTypesFactors => damagerTypesFactors;

    public void Damage(float damage, AttackType type)
    {
        // TODO: Scale attackType and redirect all damage to this script
        EventManager.Instance.Raise(new DamagePlayerEvent { damage = damage });
    }

    public void Die()
    {

    }
}
